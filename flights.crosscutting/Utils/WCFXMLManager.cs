using flights.crosscutting.DomainObjects;
using System;
using System.IO;
using System.Linq;
using System.Net;
using System.ServiceModel.Channels;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;
using System.Text.RegularExpressions;

namespace flights.crosscutting.Utils
{
    public static class WCFXMLManager
    {
        /// <summary>
        /// Retorna webResponse / data do request ao WCF
        /// </summary>
        /// <param name="urlPath"></param>
        /// <param name="soapAction"></param>
        /// <param name="query"></param>
        /// <returns></returns>
        private static HttpWebResponse GetWebResponseWCF(string urlPath, string soapAction, string query)
        {
            HttpWebRequest request;
            try
            {
                request = (HttpWebRequest)WebRequest.Create(urlPath);

                if (!string.IsNullOrEmpty(soapAction))
                {
                    request.Headers.Add("SOAPAction", soapAction);
                }

                request.ContentType = "text/xml";
                request.Accept = "text/xml";
                request.Method = "POST";


                using (Stream requestStream = request.GetRequestStream())
                {
                    byte[] byteArray = Encoding.UTF8.GetBytes(query);
                    requestStream.Write(byteArray, 0, byteArray.Length);
                    requestStream.Close();
                }

                return (HttpWebResponse)request.GetResponse();
            }
            catch (WebException exception)
            {
                throw exception;
            }
        }

        private static string BuildXMLStringFromObject(object genericObject)
        {
            try
            {
                Message messageEnvelopeRequest = Message.CreateMessage(MessageVersion.Soap11, "", genericObject);  // create a message and serialize the notifications into the message

                // Remover o HEADER
                messageEnvelopeRequest.Headers.RemoveAll("Action", "http://schemas.microsoft.com/ws/2005/05/addressing/none");

                // Transforma MensagemXml em String
                var messageReturn = messageEnvelopeRequest.ToString();

                // Remove Tag XML
                messageReturn = messageReturn.Replace("<?xml version=\"1.0\" encoding=\"utf-16\"?>", "").Trim();

                return messageReturn;
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value"></param>
        /// <returns></returns>
        public static string SerializeObjectToXmlString<T>(this T value)
        {
            if (value == null)
            {
                return string.Empty;
            }
            try
            {
                var xmlserializer = new XmlSerializer(typeof(T));
                var stringWriter = new StringWriter();
                using (var writer = XmlWriter.Create(stringWriter))
                {
                    xmlserializer.Serialize(writer, value);
                    return stringWriter.ToString();
                }
            }
            catch (Exception ex)
            {
                throw new Exception("An error occurred", ex);
            }
        }


        /// <summary>
        /// Retorna string response do WCF requisitado
        /// </summary>
        /// <param name="request"></param>
        /// <param name="url"></param>
        /// <param name="soapAction"></param>
        /// <param name="strReplace"></param>
        /// <returns></returns>
        public static string ResponseWCF(string url, string soapAction, string stringXmlRequest, string strReplace = null)
        {
            try
            {
                var xmlStringReturn = "";

                using (HttpWebResponse response = GetWebResponseWCF(url, soapAction, stringXmlRequest))
                {
                    int statusCode = (int)response.StatusCode;
                    using (StreamReader reader = new StreamReader(response.GetResponseStream()))
                    {
                        xmlStringReturn = reader.ReadToEnd();

                        xmlStringReturn = xmlStringReturn.Replace("xsi:", "");
                        Regex regex = new Regex("type=\"" + ".*?" + "\"", RegexOptions.Singleline);
                        xmlStringReturn = regex.Replace(xmlStringReturn, "");   

                        Regex regex2 = new Regex("xmlns=\"" + ".*?" + "\"", RegexOptions.Singleline);
                        xmlStringReturn = regex2.Replace(xmlStringReturn, "");

                        reader.Close();
                    }
                     response.Close();

                }

                return xmlStringReturn;
            }
            catch (WebException pse)
            {
                throw pse;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="xmlStringResponse"></param>
        /// <returns></returns>
        public static TextReader ReturnTextReaderFromXMLString(string xmlStringResponse)
        {
            TextReader reader = new StringReader(xmlStringResponse);
            return reader;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static T SerializeXmlToObject<T>(string xml) where T : class
        {
            try
            {
                using (var reader = new StringReader(xml))
                {
                    var serializer = new XmlSerializer(typeof(T));

                    return (T)serializer.Deserialize(reader);
                }
            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="xml"></param>
        /// <returns></returns>
        public static string SerializeObjectToXml<T>(T valor) where T : class
        {
            try
            {
                XmlSerializer xml = new XmlSerializer(valor.GetType());
                StringWriter retorno = new StringWriter();
                xml.Serialize(retorno, valor);
                return retorno.ToString();

            }
            catch (Exception ex)
            {
                throw new Exception(ex.Message);
            }
        }

        /// <summary>
        /// Verifica se existe valor em um elemento do xml
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="parentElement"></param>
        /// <param name="filterElementName"></param>
        /// <param name="filterElementValue"></param>
        /// <returns></returns>
        public static bool CheckIfExistValueElement(string xmlString, string parentElement, string descendants, string filterElementName, string filterElementValue)
        {
            var doc = XDocument.Parse(xmlString);
            return doc.Element(parentElement).Descendants(descendants).Where(x => x.Element(filterElementName).Value == filterElementValue).Any();
        }

        /// <summary>
        /// retorna o valor da tag no xml
        /// </summary>
        /// <param name="xmlString"></param>
        /// <param name="parentElement"></param>
        /// <param name="filterElementName"></param>
        /// <param name="filterElementValue"></param>
        /// <returns></returns>
        public static string ElementValue(string xmlString, string parentElement, string descendants)
        {
            var doc = XDocument.Parse(xmlString);
            return doc.Element(parentElement).Descendants(descendants).Select(x => x.Value).FirstOrDefault();
        }

    }
}
