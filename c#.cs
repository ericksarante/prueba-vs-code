using System;
using System.Data.SqlClient;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace ApiToSql
{
    class Program
    {
        static void Main(string[] args)
        {
            // Configuración de la conexión a la base de datos
            string connString = "Server=INYCOMHYBCQEBFE;Database=A6_INF01_InformeSeguimientoServicios;User Id=Bootcamp;Password=123456.;";

            // URL de la API y token de autenticación
            string url = "http://51.137.207.114:4000/proactivanet/api/Metrics/37ef4c35-fd65-4d9d-a86e-8798745cc2bf/metricData";
            string token = "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJEZXNhcnJvbGxvIiwib3ZyIjoiZmFsc2UiLCJhdXQiOiIwIiwibmJmIjoxNzAyNDcwNjAzLCJleHAiOjE3MzQwMDY2MDMsImlhdCI6MTcwMjQ3MDYwMywiaXNzIjoicHJvYWN0aXZhbmV0IiwiYXVkIjoiYXBpIn0.TtSMLulPr0RXxQtT4Jaz61yvyXv1LE9Yx8zxjc0Ghgc";

            // Hacer la solicitud GET a la API
            var client = new RestClient(url);
            var request = new RestRequest();
            request.AddHeader("Authorization", $"Bearer {token}");

            var response = client.Execute(request);

            if (response.StatusCode == System.Net.HttpStatusCode.OK)
            {
                // Deserializar la respuesta JSON
                JObject data = JObject.Parse(response.Content);
                JObject metricValues = (JObject)data["MetricValues"];

                // Obtener las listas de cada propiedad
                JArray etiquetas = (JArray)metricValues["etiqueta"];
                JArray resumenEtiquetas = (JArray)metricValues["ResumenEtiqueta"];
                JArray numeros = (JArray)metricValues["Numero"];
                JArray esfuerzos = (JArray)metricValues["Esfuerzos"];

                // Truncar la tabla antes de insertar los nuevos datos
                string truncateCmd = "TRUNCATE TABLE Resumen_Horas";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand comm = new SqlCommand(truncateCmd, conn))
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                }


                // Iterar sobre las listas para insertar los datos
                for (int i = 0; i < etiquetas.Count; i++)
                {
                    string etiqueta = etiquetas[i]?.ToString() ?? string.Empty;
                    string resumenEtiqueta = resumenEtiquetas[i]?.ToString() ?? string.Empty;
                    int numero = numeros[i]?.Value<int>() ?? 0;
                    decimal? esfuerzo = esfuerzos[i]?.Type == JTokenType.Null ? (decimal?)null : esfuerzos[i].Value<decimal>();

                    string insertCmd = @"INSERT INTO Resumen_Horas (Etiqueta, Numero, ResumenEtiqueta, Esfuerzos) 
                                         VALUES (@Etiqueta, @Numero, @ResumenEtiqueta, @Esfuerzos)";

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand comm = new SqlCommand(insertCmd, conn))
                        {
                            comm.Parameters.AddWithValue("@Etiqueta", etiqueta);
                            comm.Parameters.AddWithValue("@Numero", numero);
                            comm.Parameters.AddWithValue("@ResumenEtiqueta", resumenEtiqueta);
                            comm.Parameters.AddWithValue("@Esfuerzos", esfuerzo.HasValue ? (object)esfuerzo.Value : DBNull.Value);

                            conn.Open();
                            comm.ExecuteNonQuery();
                        }
                    }
                }
            }
            else
            {
                Console.WriteLine($"Error al obtener los datos de la API: {response.StatusDescription}");
                
            }
        }
    }
}
c