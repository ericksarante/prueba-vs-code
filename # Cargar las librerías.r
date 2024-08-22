# Cargar las librerías
library(httr)
library(jsonlite)
library(DBI)
library(odbc)


# Configurar la conexión a SQL Server
con <- dbConnect(odbc::odbc(),
                 Driver = "SQL Server",
                 Server = "INYCOMHYBCQEBFE",
                 Database = "A6_INF01_InformeSeguimientoServicios",
                 UID = "Bootcamp",
                 PWD = "123456.")

# Truncar la tabla
dbExecute(con, "TRUNCATE TABLE Resumen_Horas")


# URL de la API
url <- "http://51.137.207.114:4000/proactivanet/api/Metrics/37ef4c35-fd65-4d9d-a86e-8798745cc2bf/metricData"

# Token de autenticación
token <- "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9.eyJzdWIiOiJEZXNhcnJvbGxvIiwib3ZyIjoiZmFsc2UiLCJhdXQiOiIwIiwibmJmIjoxNzAyNDcwNjAzLCJleHAiOjE3MzQwMDY2MDMsImlhdCI6MTcwMjQ3MDYwMywiaXNzIjoicHJvYWN0aXZhbmV0IiwiYXVkIjoiYXBpIn0.TtSMLulPr0RXxQtT4Jaz61yvyXv1LE9Yx8zxjc0Ghgc"

# Hacer la solicitud GET con el token
response <- GET(url, add_headers(Authorization = paste("Bearer", token)))

print("hola") 