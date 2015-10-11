using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;

namespace qqq
{
    public class StockFetch
    {

        /// <summary>
        ///  used for download csv file 
        /// </summary>
        bool awk_mode = false;
        public bool AwkMode
        {
            get { return awk_mode; }
            set { awk_mode = value; }
        }
        string output_fname;
        public string OutputFileName
        {
            get { return output_fname; }
            set { output_fname = value; }
        }
        public string realtime_price()
        {
            string barchartURL = "http://www.barchart.com/quotes/stocks/HOU.TO";
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(barchartURL, @"c:\t\rrr");
            }
            catch (Exception)
            { 
            
            }
            return null;
        }

        public bool DownloadData2(string symbol, string tmpfile)
        {
            bool r = true;
            string yahooURL = //@"http://ichart.yahoo.com/table.csv?s=" + symbol;
                "http://marketdata.websol.barchart.com/getHistory.csv?key=3de188e845166054045e0234ceb2a0a0&symbol=" + symbol + "&type=daily&startDate=20000908000000";
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(yahooURL, tmpfile);

                using (System.IO.StreamWriter file =
                        new System.IO.StreamWriter(output_fname))
                {
                    List<string> lst = new List<string>();
                    using (StreamReader sr = new StreamReader(tmpfile))
                    {
                        string line;
                        // Read and display lines from the file until the end of 
                        // the file is reached.
                        while ((line = sr.ReadLine()) != null)
                        {
                            line = line.Replace("\"", "");
                            string input = Regex.Replace(line, ".*,.*,(.*,.*,.*,.*,.*,.*,.*)", "$1");
                            //Console.WriteLine(input);
                            lst.Add(input);
                        }
                        if (lst.Count <= 1) throw new Exception("Error");
                        
                        for (int i = 0; i < lst.Count; i++) {
                            file.WriteLine(lst[lst.Count - 1 - i]);
                        }

                    }
                }
            }
            catch (Exception ex)
            {
                //Console.WriteLine(ex.Message);
                r = false;
            }
            return r;
        }
        public bool DownloadData(string symbol)
        {
            bool r = true;
            string yahooURL = @"http://ichart.yahoo.com/table.csv?s=" + symbol;
            WebClient client = new WebClient();
            try
            {
                client.DownloadFile(yahooURL, output_fname);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                r = false;
            }
            return r;
        }
        public string GetQuote2(string symbol)
        {
            string result = null;
            try
            {
                string URL = "http://marketdata.websol.barchart.com/getQuote.xml?key=3de188e845166054045e0234ceb2a0a0&symbols=" + symbol;
                // Initialize a new WebRequest.
                HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(URL);
                // Get the response from the Internet resource.
                HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse();
                // Read the body of the response from the server.
                StreamReader strm =
                  new StreamReader(webresp.GetResponseStream(), Encoding.ASCII);
                //result = strm.ReadLine();
                string content = strm.ReadToEnd().Replace("\"", "");;
               // string[] contents = content.ToString().Split(',');

                result = content.Replace("\r\n", ""); ;
            
            }
            catch (Exception ex)
            { 
                Console.WriteLine (ex.Message);
            }

            return result;
        }
        // <summary>
        /// This function handles and parses multiple stock symbols as input parameters
        /// and builds a valid XML return document.
        /// </summary>
        /// <param name="symbol">A bunch of stock symbols
        ///    seperated by space or comma</param>
        /// <returns>Return stock quote data in XML format</returns>
        public string GetQuote(string symbol)
        {
            // Set the return string to null.
            string result = null;
            bool invalid_stock = false;
            try
            {
                // Use Yahoo finance service to download stock data from Yahoo
                string yahooURL = @"http://download.finance.yahoo.com/d/quotes.csv?s=" +
                                  symbol + "&f=sl1d1t1c1hgvbap2";

                //string yahooURL = @"http://ichart.yahoo.com/table.csv?s=" + symbol;
                string[] symbols = symbol.Replace(",", " ").Split(' ');

                // Initialize a new WebRequest.
                HttpWebRequest webreq = (HttpWebRequest)WebRequest.Create(yahooURL);
                // Get the response from the Internet resource.
                HttpWebResponse webresp = (HttpWebResponse)webreq.GetResponse();
                // Read the body of the response from the server.
                StreamReader strm =
                  new StreamReader(webresp.GetResponseStream(), Encoding.ASCII);

                // Construct a XML in string format.
                string tmp = "<stockquotes />";
                string content = "";
                for (int i = 0; i < symbols.Length; i++)
                {
                    // Loop through each line from the stream,
                    // building the return XML Document string
                    if (symbols[i].Trim() == "")
                        continue;

                    content = strm.ReadLine().Replace("\"", "");
                    string[] contents = content.ToString().Split(',');
                    // If contents[2] = "N/A". the stock symbol is invalid.
                    if (contents[2] == "N/A")
                    {
                        invalid_stock = true;
                    }
                    else
                    {
                        produce_xml_line(contents, ref tmp);
                        if (awk_mode)
                        {
                            Console.WriteLine("last: " + contents[1] + " date: " + contents[2] + " time:" + contents[3]);
                        }
                    }
                    // Set the return string
                    result += tmp;
                    tmp = "";
                }
                // Set the return string
                result += "</StockQuotes>";
                // Close the StreamReader object.
                strm.Close();
            }
            catch
            {
                // Handle exceptions.
            }
            if (invalid_stock == true) return null;
            // Return the stock quote data in XML format.
            return result;
        }

        void produce_xml_line(string[] contents, ref string tmp)
        {
            //construct XML via strings.
            tmp += "<Stock>";
            tmp += "<Symbol>" + contents[0] + "</Symbol>";
            try
            {
                tmp += "<Last>" +
                  String.Format("{0:c}", Convert.ToDouble(contents[1])) +
                                "</Last>";
            }
            catch
            {
                tmp += "<Last>" + contents[1] + "</Last>";
            }
            tmp += "<Date>" + contents[2] + "</Date>";
            tmp += "<Time>" + contents[3] + "</Time>";
            // "<" and ">" are illegal in XML elements.
            // Replace the characters "<" and ">"
            // to "&gt;" and "&lt;".
            if (contents[4].Trim().Substring(0, 1) == "-")
                tmp += "<Change>&lt;span style='color:red'&gt;" +
                       contents[4] + "(" + contents[10] + ")" +
                       "&lt;span&gt;</Change>";
            else if (contents[4].Trim().Substring(0, 1) == "+")
                tmp += "<Change>&lt;span style='color:green'&gt;" +
                       contents[4] + "(" + contents[10] + ")" +
                       "&lt;span&gt;</Change>";
            else
                tmp += "<Change>" + contents[4] + "(" +
                       contents[10] + ")" + "</Change>";
            tmp += "<High>" + contents[5] + "</High>";
            tmp += "<Low>" + contents[6] + "</Low>";
            try
            {
                tmp += "<Volume>" + String.Format("{0:0,0}",
                       Convert.ToInt64(contents[7])) + "</Volume>";
            }
            catch
            {
                tmp += "<Volume>" + contents[7] + "</Volume>";
            }
            tmp += "<Bid>" + contents[8] + "</Bid>";
            tmp += "<Ask>" + contents[9] + "</Ask>";
            tmp += "</Stock>";

        }
        void constuct_xml_line(ref string tmp, string[] symbols, int i)
        {
            // Construct XML via strings.
            tmp += "<Stock>";
            // "<" and ">" are illegal
            // in XML elements. Replace the characters "<"
            // and ">" to "&gt;" and "&lt;".
            tmp += "<Symbol>&lt;span style='color:red'&gt;" +
                   symbols[i].ToUpper() +
                   " is invalid.&lt;/span&gt;</Symbol>";
            tmp += "<Last></Last>";
            tmp += "<Date></Date>";
            tmp += "<Time></Time>";
            tmp += "<Change></Change>";
            tmp += "<High></High>";
            tmp += "<Low></Low>";
            tmp += "<Volume></Volume>";
            tmp += "<Bid></Bid>";
            tmp += "<Ask></Ask>";
            tmp += "<Ask></Ask>";
            tmp += "</Stock>";
        }
    }

}
