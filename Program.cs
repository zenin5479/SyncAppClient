using System;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;

namespace SyncAppClient
{
   internal class Program
   {
      static void Main()
      {
         Console.WriteLine("Тестирование HTTP сервера...");
         string baseUrl = "http://localhost:8080/";
         try
         {
            // Тестирование GET запроса
            Console.WriteLine("=== ТЕСТ 1: GET запрос ===");
            TestGetRequest(baseUrl + "?name=TestUser");

            // Тестирование POST запроса
            Console.WriteLine("=== ТЕСТ 2: POST запрос ===");
            TestPostRequest(baseUrl);

            // Тестирование PUT запроса
            Console.WriteLine("=== ТЕСТ 3: PUT запрос ===");
            TestPutRequest(baseUrl);

            // Тестирование DELETE запроса
            Console.WriteLine("=== ТЕСТ 4: DELETE запрос ===");
            TestDeleteRequest(baseUrl + "resource/123");

            // Тестирование с параметрами
            Console.WriteLine("=== ТЕСТ 5: GET с разными параметрами ===");
            TestGetWithParameters(baseUrl);

            // Тестирование ошибок
            Console.WriteLine("=== ТЕСТ 6: Неподдерживаемый метод ===");
            TestUnsupportedMethod(baseUrl);
         }
         catch (Exception ex)
         {
            Console.WriteLine("Ошибка при тестировании: {0}", ex.Message);
         }

         Console.WriteLine("Нажмите любую клавишу для выхода...");

         Console.ReadKey();
      }

      static void TestGetRequest(string url)
      {
         using (WebClient client = new WebClient())
         {
            try
            {
               Console.WriteLine("Отправка GET на: {0}", url);
               string response = client.DownloadString(url);
               Console.WriteLine("Ответ: {0}", response);
               Console.WriteLine("Статус: УСПЕХ");
            }
            catch (WebException ex)
            {
               Console.WriteLine("Ошибка: {0}", ex.Message);
               if (ex.Response is HttpWebResponse httpResponse)
               {
                  Console.WriteLine("Код статуса: {0}", httpResponse.StatusCode);
               }
            }
         }
      }

      static void TestPostRequest(string url)
      {
         using (WebClient client = new WebClient())
         {
            try
            {
               Console.WriteLine("Отправка POST на: {0}", url);

               // Устанавливаем заголовки
               client.Headers[HttpRequestHeader.ContentType] = "application/json";
               client.Encoding = Encoding.UTF8;
               // Тестовые данные
               string jsonData = "{\"name\":\"Test\", \"value\":123}";
               Console.WriteLine("Отправляемые данные: {0}", jsonData);
               // Отправляем запрос
               string response = client.UploadString(url, "POST", jsonData);
               Console.WriteLine("Ответ: {0}", response);
               Console.WriteLine("Статус: УСПЕХ");
            }
            catch (WebException ex)
            {
               Console.WriteLine("Ошибка: {0}", ex.Message);
               if (ex.Response is HttpWebResponse httpResponse)
               {
                  Console.WriteLine("Код статуса: {0}", httpResponse.StatusCode);
                  // Читаем тело ответа с ошибкой
                  using (Stream stream = ex.Response.GetResponseStream())
                  {
                     using (StreamReader reader = new StreamReader(stream))
                     {
                        string errorResponse = reader.ReadToEnd();
                        Console.WriteLine("Ответ сервера: {0}", errorResponse);
                     }
                  }
               }
            }
         }
      }

      static void TestPutRequest(string url)
      {
         using (WebClient client = new WebClient())
         {
            try
            {
               Console.WriteLine("Отправка PUT на: {0}", url);
               client.Headers[HttpRequestHeader.ContentType] = "application/json";
               client.Encoding = Encoding.UTF8;
               string jsonData = "{\"id\":456, \"name\":\"UpdatedItem\"}";
               Console.WriteLine("Отправляемые данные: {0}", jsonData);
               string response = client.UploadString(url, "PUT", jsonData);
               Console.WriteLine("Ответ: {0}", response);
               Console.WriteLine("Статус: УСПЕХ");
            }
            catch (WebException ex)
            {
               Console.WriteLine("Ошибка: {0}", ex.Message);
               if (ex.Response is HttpWebResponse httpResponse)
               {
                  Console.WriteLine("Код статуса: {0}", httpResponse.StatusCode);
               }
            }
         }
      }

      static void TestDeleteRequest(string url)
      {
         using (WebClient client = new WebClient())
         {
            try
            {
               Console.WriteLine("Отправка DELETE на: {0}", url);
               // Для DELETE обычно не отправляем тело
               byte[] responseBytes = client.UploadData(url, "DELETE", new byte[0]);
               string response = Encoding.UTF8.GetString(responseBytes);
               Console.WriteLine("Ответ: {0}", response);
               Console.WriteLine("Статус: УСПЕХ");
            }
            catch (WebException ex)
            {
               Console.WriteLine("Ошибка: {0}", ex.Message);
               if (ex.Response is HttpWebResponse httpResponse)
               {
                  Console.WriteLine("Код статуса: {0}", httpResponse.StatusCode);
                  Console.WriteLine("Описание статуса: {0}", httpResponse.StatusDescription);
               }
            }
         }
      }

      static void TestGetWithParameters(string baseUrl)
      {
         using (WebClient client = new WebClient())
         {
            try
            {
               // Тест 1: Без параметров
               Console.WriteLine("1. GET без параметров:");
               string response1 = client.DownloadString(baseUrl);
               Console.WriteLine("Ответ: {0}", response1);

               // Тест 2: С параметром name
               Console.WriteLine("2. GET с параметром name=Alice:");
               string urlWithParam = string.Format("{0}?name=Alice&age=30", baseUrl);
               string response2 = client.DownloadString(urlWithParam);
               Console.WriteLine("Ответ: {0}", response2);

               // Тест 3: С несколькими параметрами
               Console.WriteLine("3. GET с несколькими параметрами:");
               NameValueCollection query = new NameValueCollection();
               query["name"] = "Bob";
               query["city"] = "Moscow";
               query["lang"] = "ru";

               string queryString = ToQueryString(query);
               string response3 = client.DownloadString(baseUrl + "?" + queryString);
               Console.WriteLine("Ответ: {0}", response3);
            }
            catch (WebException ex)
            {
               Console.WriteLine("Ошибка: {0}", ex.Message);
            }
         }
      }

      static void TestUnsupportedMethod(string url)
      {
         using (WebClient client = new WebClient())
         {
            try
            {
               Console.WriteLine("Отправка PATCH на: {0}", url);
               // Пытаемся отправить неподдерживаемый метод
               string jsonData = "{\"test\":\"data\"}";
               client.Headers[HttpRequestHeader.ContentType] = "application/json";
               // PATCH не поддерживается нашим сервером
               string response = client.UploadString(url, "PATCH", jsonData);
               Console.WriteLine("Ответ: {0}", response);
            }
            catch (WebException ex)
            {
               Console.WriteLine("Ожидаемая ошибка: {0}", ex.Message);
               if (ex.Response is HttpWebResponse httpResponse)
               {
                  Console.WriteLine("Код статуса: {0} ({1})", httpResponse.StatusCode, (int)httpResponse.StatusCode);
                  Console.WriteLine("Статус: {0}", httpResponse.StatusCode == HttpStatusCode.MethodNotAllowed ? "ПРАВИЛЬНО" : "НЕПРАВИЛЬНО");
               }
            }
         }
      }

      static string ToQueryString(NameValueCollection nvc)
      {
         string Converter(string key)
         {
            return string.Format("{0}={1}", Uri.EscapeDataString(key), Uri.EscapeDataString(nvc[key]));
         }

         string[] array = Array.ConvertAll(nvc.AllKeys, Converter);
         return string.Join("&", array);
      }
   }
}