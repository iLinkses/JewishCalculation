using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Text.RegularExpressions;
using System.Windows;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;

namespace JewishCalculationWPF.Classes
{
    class GetProductsFromCheck
    {
        internal bool GetCheck(string fiscal_mark, string state_number, double sum, DateTime dateTime, string num_fiscal_doc = "")
        {
            if (GetCountry().Equals("Russia"))
            {
                if (GetCheckFromOfdRu(dateTime, sum, fiscal_mark, num_fiscal_doc, state_number)) return true;
                else return false;
            }
            if (GetCountry().Equals("Kazakhstan"))
            {
                if (GetCheckFromConsumer(fiscal_mark, state_number, sum, dateTime))
                {
                    return true;
                }
                else if (GetCheckFromOfd1(dateTime, state_number, fiscal_mark))
                {
                    return true;
                }
                else return false;
            }
            else return false;
        }
        /// <summary>
        /// Определяет метонахождение по ip
        /// </summary>
        /// <returns>Название страны</returns>
        private string GetCountry()
        {
            var locationResponse = new WebClient().DownloadString($"https://ipwhois.app/json/{(new WebClient().DownloadString("https://api.ipify.org"))}");
            JObject whoisJO = JObject.Parse(locationResponse);
            return whoisJO["country"].ToString();
        }
        /// <summary>
        /// Заполнение товаров по чеку (ВНИМАНИЕ!!! Работает только для казахтелекома consumer.oofd.kz)
        /// </summary>
        /// <param name="fiscal_mark">ФН чека</param>
        /// <param name="state_number">РНМ чека</param>
        /// <param name="sum">Сумма чека</param>
        /// <param name="dateTime">дата чека</param>
        internal bool GetCheckFromConsumer(string fiscal_mark, string state_number, double sum, DateTime dateTime)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create($"https://consumer.oofd.kz?i={fiscal_mark}&f={state_number}&s={sum}&t={dateTime:yyyyMMdd}T{dateTime:HHmmss}");
            httpRequest.AllowAutoRedirect = false;
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            if (httpResponse.Headers.Get("Location").Contains("/ticket-not-found"))
            {
                return false;
            }

            using (var webClient = new WebClient())
            {
                var json = webClient.DownloadData($"https://consumer.oofd.kz/api/tickets{httpResponse.Headers.Get("Location")}");
                JObject o = JObject.Parse(Encoding.UTF8.GetString(json));
                var value = o["ticket"]["items"].Select(i => i["commodity"]).Select(i => new
                {
                    name = i["name"],
                    price = i["price"],
                    quantity = i["quantity"],
                    sum = i["sum"]
                }).ToList();
                foreach (var t in value)
                {
                    Models.Products.Add(new Models.Product
                    {
                        Name = t.name.ToString(),
                        Price = double.Parse(t.price.ToString()),
                        Quantity = double.Parse(t.quantity.ToString()),
                        Sum = double.Parse(t.sum.ToString())
                    });
                }
                return true;
            }

        }
        /// <summary>
        /// Заполнение товаров по чеку (ВНИМАНИЕ!!! Работает только для ofd1.kz)
        /// </summary>
        /// <param name="date">Дата на чеке</param>
        /// <param name="state_number">РНМ чека</param>
        /// <param name="fiscal_mark">ФН чека</param>
        internal bool GetCheckFromOfd1(DateTime date, string state_number, string fiscal_mark)
        {
            var httpRequest = (HttpWebRequest)WebRequest.Create("https://ofd1.kz/check_ticket");
            httpRequest.Method = "POST";

            httpRequest.Headers["cookie"] = "_pulsar_key=SFMyNTY.g3QAAAABbQAAAAtfY3NyZl90b2tlbm0AAAAYMXk2ZmlNOU5xcHlUMWNfbUpIdWRIY01W.XUhxZyFBKc12WbHPm-aCwCD3Z_Niv6BOu8DnK4D2_Ak";
            httpRequest.ContentType = "application/x-www-form-urlencoded";

            var data = $"_csrf_token=Uy9XKC4SeCQZRikeYVQqOnkqNiIODXUEbVaNG_Ajh6PJP7uW3bCFFn8R&_utf8=%25E2%259C%2593&date={date:dd.MM.yyyy}&state_number={state_number}&fiscal_mark={fiscal_mark}";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            string html;
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                html = streamReader.ReadToEnd();
            }

            HtmlDocument doc = new HtmlDocument();
            doc.LoadHtml(html);

            ///Проверка на существование чека
            var allertNodes = doc.DocumentNode.SelectNodes("//p[@class=\"alert alert-danger\"]");
            if (allertNodes != null)
            {
                foreach (var node in allertNodes)
                {
                    if (node.InnerText != "")
                    {
                        Console.WriteLine($"{node.InnerHtml}");
                        return false;
                    }
                }

            }

            var nodes = doc.DocumentNode.SelectNodes("//ol[@class=\"ready_ticket__items_list\"]//li");
            if (nodes == null)
                throw new ArgumentNullException("Данные не корректны");

            foreach (var node in doc.DocumentNode.SelectNodes("//ol[@class=\"ready_ticket__items_list\"]//li"))
            {
                string ready_ticket__item = "";
                foreach (var subnode in node.SelectNodes("div[@class =\"ready_ticket__item\"]/text()"))
                {
                    if (Regex.IsMatch(subnode.InnerText, "[\\S]+"))
                    {
                        ready_ticket__item += Regex.Replace(subnode.InnerText.Replace("\r\n", " ").Replace("\n", " ").Trim(), "[ ]+", " ");
                    }
                    else continue;
                }

                Models.Products.Add(new Models.Product
                {
                    Name = node.SelectSingleNode("text()").InnerText.Trim(),
                    Price = double.Parse(ready_ticket__item.Substring(0, ready_ticket__item.IndexOf('x')).Trim(), CultureInfo.InvariantCulture),
                    Quantity = double.Parse(new Regex(@"x(.*?)=").Match(ready_ticket__item).Groups[1].Value.Trim(), CultureInfo.InvariantCulture),
                    Sum = double.Parse(new Regex(@"=(.*)").Match(ready_ticket__item).Groups[1].Value.Trim(), CultureInfo.InvariantCulture)
                });
            }
            return true;
        }
        /// <summary>
        /// Заполнение товаров по чеку. Работает только в России
        /// </summary>
        /// <param name="tCh">Дата чека</param>
        /// <param name="sCh">Итоговая сумма чека</param>
        /// <param name="fnCh">ФН. Номер фискального накопителя</param>
        /// <param name="iCh">ФД. Номер фискального документа</param>
        /// <param name="fpCh">ФП. Фискальный признак документа</param>
        internal bool GetCheckFromOfdRu(DateTime tCh, double sCh, string fnCh, string iCh, string fpCh)
        {
            var url = "https://proverkacheka.com/api/v1/check/get";//"https://proverkacheka.com/check/get";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/x-www-form-urlencoded";

            var data = $"t={tCh:yyyyMMdd}T{tCh:HHmm}&s={sCh}&fn={fnCh}&i={iCh}&fp={fpCh}&n=1";

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();
            using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
            {
                var json = streamReader.ReadToEnd();
                JObject jO = JObject.Parse(json);
                if (jO["code"].ToString().Equals("1"))
                {
                    var value = jO["data"]["json"]["items"].Select(i => new
                    {
                        name = i["name"],
                        price = i["price"],
                        quantity = i["quantity"],
                        sum = i["sum"]
                    }).ToList();
                    foreach (var t in value)
                    {
                        Models.Products.Add(new Models.Product
                        {
                            Name = t.name.ToString(),
                            Price = double.Parse(t.price.ToString().Insert(t.price.ToString().Length - 2, ",")),    //В json-е приходили числа без разделителя дробной части.
                            Quantity = double.Parse(t.quantity.ToString()),
                            Sum = double.Parse(t.sum.ToString().Insert(t.sum.ToString().Length - 2, ","))   //Если что, может упасть на этом месте. Лучше сделать проверку на уже существование разделителя.
                        });
                    }
                    return true;
                }
                else return false;
            }
        }
    }

    abstract class FromCheck { }

    class FromCheckKZ : FromCheck
    {

    }

    class FromCheckRU : FromCheck
    {
        public bool Done { get; private set; }
        public FromCheckRU(DateTime tCh, double sCh, string fnCh, string iCh, string fpCh)
        {
            var url = "https://check.ofd.ru/Document/FetchReceiptFromFns";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.Method = "POST";

            httpRequest.ContentType = "application/json";

            var data = JsonConvert.SerializeObject(new
            {
                TotalSum = sCh,
                FnNumber = fnCh,
                ReceiptOperationType = "1",
                DocNumber = iCh,
                DocFiscalSign = fpCh,
                DocDateTime = tCh
            });

            using (var streamWriter = new StreamWriter(httpRequest.GetRequestStream()))
            {
                streamWriter.Write(data);
            }

            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            if (httpResponse.StatusCode == HttpStatusCode.OK)
            {
                using (var streamReader = new StreamReader(httpResponse.GetResponseStream()))
                {
                    var Content = streamReader.ReadToEnd();
                    HtmlDocument doc = new HtmlDocument();
                    doc.LoadHtml(Content);

                    ///получаем ноды между комментариев
                    var tovarNodes = doc.DocumentNode.SelectNodes("//*[preceding::comment()[.=\"<!-- Products -->\"]][following::comment()[.=\"<!-- /Products -->\"]]/div[@class=\"ifw-cols ifw-cols-2\"]");//Работает!!! ну наканецта!

                    foreach (var nodes in tovarNodes)
                    {
                        string Data = Regex.Replace(nodes.SelectNodes("div[@class=\"ifw-col ifw-col-1 text-right\"]").FirstOrDefault().InnerText.Replace("\r\n", ""), "[ ]+", " ").Trim();
                        Models.Products.Add(new Models.Product
                        {
                            Name = nodes.SelectNodes("div[@class=\"ifw-col ifw-col-1 text-left\"]").FirstOrDefault().InnerText.Replace("&quot;", ""),
                            Price = double.Parse(new Regex(@"X(.*?)=").Match(Data).Groups[1].Value.Trim(), CultureInfo.InvariantCulture),
                            Quantity = double.Parse(Data.Substring(0, Data.IndexOf('X')).Trim(), CultureInfo.InvariantCulture),
                            Sum = double.Parse(new Regex(@"=(.*)").Match(Data).Groups[1].Value.Trim(), CultureInfo.InvariantCulture)
                        });
                    }
                }
                Done = true;
            }
            else Done = false;
        }
    }

    abstract class Check
    {
        public abstract bool Done { get; set; }
        public abstract FromCheck GetFromCheck();
    }

    class CheckFromCheckKZ : Check
    {
        public override bool Done { get; set; }
        public override FromCheck GetFromCheck()
        {
            return new FromCheckKZ();
        }
    }

    class CheckFromCheckRU : Check
    {
        public override bool Done { get; set; }

        private DateTime DateTime { get; set; }
        private double Sum { get; set; }
        private string Fiscal_mark { get; set; }
        private string Num_fiscal_doc { get; set; }
        private string State_number { get; set; }
        public CheckFromCheckRU(string fiscal_mark, string state_number, double sum, DateTime dateTime, string num_fiscal_doc = "")
        {
            DateTime = dateTime;
            Sum = sum;
            Fiscal_mark = fiscal_mark;
            Num_fiscal_doc = num_fiscal_doc;
            State_number = state_number;
        }
        public override FromCheck GetFromCheck()
        {
            FromCheckRU fromCheckRU = new FromCheckRU(DateTime, Sum, Fiscal_mark, Num_fiscal_doc, State_number);
            Done = fromCheckRU.Done;
            return fromCheckRU;
        }
    }
}
