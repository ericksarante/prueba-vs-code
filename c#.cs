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
            string url = "http://51.137.207.114:4000/proactivanet/api/Metrics/46c986d4-24f5-477b-a729-e43b9b963e68/metricData";
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

                // Mapeo de los nombres en el JSON a los nombres en la tabla SQL
                JArray grupos = (JArray)metricValues["Grupo"];
                JArray horasREQ = (JArray)metricValues["Horas REQ"];
                JArray tasREQ = (JArray)metricValues["TAS REQ"];
                JArray horasINC = (JArray)metricValues["Horas INC"];
                JArray tasINC = (JArray)metricValues["TAS INC"];
                JArray horasPRO = (JArray)metricValues["Horas PRO"];
                JArray tasPRO = (JArray)metricValues["TAS PRO"];
                JArray horasCAM = (JArray)metricValues["Horas CAM"];
                JArray tasCAM = (JArray)metricValues["TAS CAM"];
                JArray horasTPI = (JArray)metricValues["Horas TPI"];
                JArray tasTPI = (JArray)metricValues["TAS TPI"];
                JArray horasTPR = (JArray)metricValues["Horas TPR"];
                JArray tasTPR = (JArray)metricValues["TAS TPR"];

                // Truncar la tabla antes de insertar los nuevos datos
                string truncateCmd = "TRUNCATE TABLE Resumen_GrupoSoporte_panet";
                using (SqlConnection conn = new SqlConnection(connString))
                {
                    using (SqlCommand comm = new SqlCommand(truncateCmd, conn))
                    {
                        conn.Open();
                        comm.ExecuteNonQuery();
                    }
                }

                // Iterar sobre las listas para insertar los datos
                for (int i = 0; i < grupos.Count; i++)
                {
                    string grupo = grupos[i]?.ToString() ?? string.Empty;
                    decimal? horasREQValue = GetDecimalValue(horasREQ, i);
                    decimal? tasREQValue = GetDecimalValue(tasREQ, i);
                    decimal? horasINCValue = GetDecimalValue(horasINC, i);
                    decimal? tasINCValue = GetDecimalValue(tasINC, i);
                    decimal? horasPROValue = GetDecimalValue(horasPRO, i);
                    decimal? tasPROValue = GetDecimalValue(tasPRO, i);
                    decimal? horasCAMValue = GetDecimalValue(horasCAM, i);
                    decimal? tasCAMValue = GetDecimalValue(tasCAM, i);
                    decimal? horasTPIValue = GetDecimalValue(horasTPI, i);
                    decimal? tasTPIValue = GetDecimalValue(tasTPI, i);
                    decimal? horasTPRValue = GetDecimalValue(horasTPR, i);
                    decimal? tasTPRValue = GetDecimalValue(tasTPR, i);

                    string insertCmd = @"INSERT INTO Resumen_GrupoSoporte_panet 
                        ([Grupo], [Horas_REQ], [TAS_REQ], [Horas_INC], [TAS_INC], [Horas_PRO], [TAS_PRO], [Horas_CAM], [TAS_CAM], [Horas_TPI], [TAS_TPI], [Horas_TPR], [TAS_TPR]) 
                        VALUES 
                        (@Grupo, @HorasREQ, @TASREQ, @HorasINC, @TASINC, @HorasPRO, @TASPRO, @HorasCAM, @TASCAM, @HorasTPI, @TASTPI, @HorasTPR, @TASTPR)";

                    using (SqlConnection conn = new SqlConnection(connString))
                    {
                        using (SqlCommand comm = new SqlCommand(insertCmd, conn))
                        {
                            comm.Parameters.AddWithValue("@Grupo", grupo);
                            comm.Parameters.AddWithValue("@HorasREQ", horasREQValue.HasValue ? (object)horasREQValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@TASREQ", tasREQValue.HasValue ? (object)tasREQValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@HorasINC", horasINCValue.HasValue ? (object)horasINCValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@TASINC", tasINCValue.HasValue ? (object)tasINCValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@HorasPRO", horasPROValue.HasValue ? (object)horasPROValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@TASPRO", tasPROValue.HasValue ? (object)tasPROValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@HorasCAM", horasCAMValue.HasValue ? (object)horasCAMValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@TASCAM", tasCAMValue.HasValue ? (object)tasCAMValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@HorasTPI", horasTPIValue.HasValue ? (object)horasTPIValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@TASTPI", tasTPIValue.HasValue ? (object)tasTPIValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@HorasTPR", horasTPRValue.HasValue ? (object)horasTPRValue.Value : DBNull.Value);
                            comm.Parameters.AddWithValue("@TASTPR", tasTPRValue.HasValue ? (object)tasTPRValue.Value : DBNull.Value);

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

        // Método auxiliar para obtener un valor decimal seguro
        static decimal? GetDecimalValue(JArray array, int index)
        {
            if (array != null && array.Count > index && array[index] != null && array[index].Type != JTokenType.Null)
            {
                return array[index].Value<decimal>();
            }
            return null;
        }
    }
}
