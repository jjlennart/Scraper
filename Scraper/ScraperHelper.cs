using Entities;
using HtmlAgilityPack;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;

namespace Scraper
{
    public class ScraperHelper
    {
        public ScraperHelper()
        {
            string urlAddress = "http://www.j-archive.com/showgame.php?game_id=6136";
            // var html = GetHtml(urlAddress);
            Agility(urlAddress);
        }

        public string GetHtml(string urlAddress)
        {


            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(urlAddress);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();

            if (response.StatusCode == HttpStatusCode.OK)
            {
                Stream receiveStream = response.GetResponseStream();
                StreamReader readStream = null;

                if (response.CharacterSet == null)
                {
                    readStream = new StreamReader(receiveStream);
                }
                else
                {
                    readStream = new StreamReader(receiveStream, Encoding.GetEncoding(response.CharacterSet));
                }

                string data = readStream.ReadToEnd();

                response.Close();
                readStream.Close();
                return data;
            }
            return null;

        }

        public void Agility(string url)
        {
            var web = new HtmlWeb();
            var htmlDoc = web.Load(url);
            int numberOfCategories = 6;
            int numberOfCluesPerCategory = 5;

            var htmlBody = htmlDoc.DocumentNode.SelectSingleNode("//body");

            HtmlNodeCollection childNodes = htmlBody.ChildNodes;
            var game = new Game();
            var categories = new List<Category>();
            var clues = new List<Clue>();
            for (int i = 0; i < numberOfCategories; i++)
            {
                categories.Add(new Category
                {
                    Name = htmlBody.SelectNodes("//div[@id='jeopardy_round']//td[contains(@class, 'category')]//td[contains(@class, 'category_name')]")[i].InnerHtml,
                    Comment = htmlBody.SelectNodes("//div[@id='jeopardy_round']//td[contains(@class, 'category')]//td[contains(@class, 'category_comments')]")[i].InnerHtml,
                    Game = game,
                    Round = null
                });
            }

            for (int k = 0; k < numberOfCluesPerCategory; k++)

            {
                for (int j = 0; j < numberOfCategories; j++)
                {
                    var clueNumber = numberOfCluesPerCategory * k + k  + j;
                    var clueValue = htmlBody.SelectNodes("//div[@id='jeopardy_round']//td[contains(@class, 'clue')]//td[contains(@class, 'clue_value')]")[clueNumber].InnerHtml;

                    //TODO: If clue not shown, record some how
                    clues.Add(new Clue
                    {
                        Category = categories[j],
                        Value = (k+1) * 200,
                        IsDailyDouble = clueValue.Contains("DD"),
                        ClueText = htmlBody.SelectNodes("//div[@id='jeopardy_round']//td[contains(@class, 'clue')]//td[contains(@class, 'clue_text')]")[clueNumber].InnerHtml,
                        Response = GetResponseFromString(htmlBody.SelectNodes("//div[@id='jeopardy_round']//td[contains(@class, 'clue')]//div")[clueNumber].OuterHtml),
                });

                   
                }



            }
           
        }
        public string GetResponseFromString(string html)
        {
            var startString = "correct_response&quot;&gt;";
            var endString = "&lt;/em&gt;&lt;br";
            int pFrom = html.IndexOf(startString) + startString.Length;
            int pTo = html.LastIndexOf(endString);

            return StripHTML(HttpUtility.HtmlDecode(html.Substring(pFrom, pTo - pFrom)));
        }

        public static string StripHTML(string input)
        {
            return Regex.Replace(input, "<.*?>", String.Empty);
        }
    }
}
