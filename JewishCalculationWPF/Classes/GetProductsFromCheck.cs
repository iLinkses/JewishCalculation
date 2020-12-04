using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Newtonsoft.Json.Linq;

namespace JewishCalculationWPF.Classes
{
    class GetProductsFromCheck
    {
        internal bool GetCheck(string fiscal_mark, string state_number, double sum, DateTime dateTime)
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
        /// <summary>
        /// Заполнение товаров по чеку (ВНИМАНИЕ!!! Работает только для казахтелекома consumer.oofd.kz)
        /// </summary>
        /// <param name="fiscal_mark">ФН чека</param>
        /// <param name="state_number">РНМ чека</param>
        /// <param name="sum">Сумма чека</param>
        /// <param name="dateTime">дата чека</param>
        internal bool GetCheckFromConsumer(string fiscal_mark, string state_number, double sum, DateTime dateTime)
        {
            var url = $"https://consumer.oofd.kz?i={fiscal_mark}&f={state_number}&s={sum}&t={dateTime:yyyyMMdd}T{dateTime:HHmmss}";
            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
            httpRequest.AllowAutoRedirect = false;
            var httpResponse = (HttpWebResponse)httpRequest.GetResponse();

            if (httpResponse.Headers.Get("Location").Contains("/ticket-not-found"))
            {
                //GetCheckFromOfd1(dateTime, state_number, fiscal_mark);
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
                    Models.products.Add(new Models.Product
                    {
                        Name = t.name.ToString(),
                        Price = double.Parse(t.price.ToString()),
                        Quantity = double.Parse(t.quantity.ToString())
                    });
                    //Console.WriteLine($"Название: {t.name};\nЦена: {t.price};\nКоличество: {t.quantity};\nСумма: {t.sum}\n\n");
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

            //20.08.2020
            var url = "https://ofd1.kz/check_ticket";

            var httpRequest = (HttpWebRequest)WebRequest.Create(url);
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
                    if (node.InnerText.Contains("Чек не найден.."))
                    {
                        Console.WriteLine($"{node.InnerHtml}");
                        return false;
                    }
                }
            }

            //получаем список всех span в которых содержится цена
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

                //string Name = node.SelectSingleNode("text()").InnerText.Trim();
                //string P = ready_ticket__item.Substring(0, ready_ticket__item.IndexOf('x')).Trim();
                //string Q = new Regex(@"x(.*?)=").Match(ready_ticket__item).Groups[1].Value.Trim();
                //double Price = double.Parse(P, CultureInfo.InvariantCulture);
                //double Quantity = double.Parse(Q, CultureInfo.InvariantCulture);

                Models.products.Add(new Models.Product
                {
                    Name = node.SelectSingleNode("text()").InnerText.Trim(),
                    Price = double.Parse(ready_ticket__item.Substring(0, ready_ticket__item.IndexOf('x')).Trim(), CultureInfo.InvariantCulture),
                    Quantity = double.Parse(new Regex(@"x(.*?)=").Match(ready_ticket__item).Groups[1].Value.Trim(), CultureInfo.InvariantCulture)
                    //sum = new Regex(@"=(.*)").Match(ready_ticket__item).Groups[1].Value.Trim()
                });
            }
            return true;
        }
    }
}
