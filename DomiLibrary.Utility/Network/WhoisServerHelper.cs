﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Net.Sockets;
using System.Text;
using DomiLibrary.Utility.Helper;

namespace DomiLibrary.Utility.Network
{
    public class WhoisServerHelper
    {
        public static char[] IgnoreCharacters = new[] {';', ',', ':', ' '};

        /// <summary>
        /// Gets the whois information.
        /// http://dotnet-snippets.com/dns/gets-the-whois-information-SID581.aspx
        /// Author: Jan Welker 
        /// </summary>
        /// <param name="whoisServer">The whois server.</param>
        /// <param name="url">The URL.</param>
        /// <returns></returns>
        public static string GetWhoisInformation(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var stringBuilderResult = new StringBuilder();
            var tcpClinetWhois = new TcpClient(whoisServer, 43);
            var networkStreamWhois = tcpClinetWhois.GetStream();
            var bufferedStreamWhois = new BufferedStream(networkStreamWhois);
            var streamWriter = new StreamWriter(bufferedStreamWhois);

            streamWriter.WriteLine(url);
            streamWriter.Flush();

            var streamReaderReceive = new StreamReader(bufferedStreamWhois);

            try
            {
                while (!streamReaderReceive.EndOfStream)
                    stringBuilderResult.AppendLine(streamReaderReceive.ReadLine());

                return stringBuilderResult.ToString();
            }
            catch (Exception) 
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Funcion que devuelve la informacion whois al completo, con los datos de contacto
        /// </summary>
        /// <param name="whoisServer">Whois server</param>
        /// <param name="url">Url</param>
        /// <returns>Informacion del whois</returns>
        public static string GetWhoisInformationComplete(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var whois = GetWhoisInformation(whoisServer, url);
            var whoisServerName = GetWhoisServer(whoisServer, url);
            if(whoisServerName != null && !whoisServerName.Equals(string.Empty))
            {
                whois += GetWhoisInformation(whoisServerName, url);
            }

            return whois;
        }

        /// <summary>
        /// Funcion que devuelve el valor de un dominio pasado una propiedad
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <param name="property">propiedad de busqueda</param>
        /// <param name="separator">separador</param>
        /// <returns>Listado de propiedades del whois</returns>
        public static IList<string> GetValueByProperty(string whoisServer, string url, string property, string separator)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);
            ValidationHelper.NotBlank(property);

            var whois = GetWhoisInformation(whoisServer, url);
            if(whois == null || whois.Equals(string.Empty))
                return new List<string>();

            var nameServers = StringHelper.SearchStringGetValue(whois, property, separator, true);

            return nameServers;
        }

        /// <summary>
        /// Devuelve el nombre del dominio registado
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>Retorna el nombre del dominio</returns>
        public static string GetDomainName(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Domain Name:", "\r\n");

            if(values != null && values.Count>0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }

        /// <summary>
        /// Retorna el servidor whois que usa para resolver la peticion
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>Retorna el whois server</returns>
        public static string GetWhoisServer(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Whois Server:", "\r\n");

            if (values != null && values.Count > 0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }

        /// <summary>
        /// Retorna la url de referencia
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>retorna url referencia</returns>
        public static string GetReferralUrl(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Referral URL:", "\r\n");

            if (values != null && values.Count > 0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }

        /// <summary>
        /// Retorna los nombre de servidor asociado al dominio
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>retorna el nombre del servidor NS</returns>
        public static IList<string> GetNameServer(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Name Server:", "\r\n");

            for (var i = 0; i < values.Count; i++)
            {
                values[i] = values[i].Trim(IgnoreCharacters);
            }

            return values;
        }

        /// <summary>
        /// Retorna el estado del dominio
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>retorna el estado</returns>
        public static string GetStatus(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Status:", "\r\n");

            if (values != null && values.Count > 0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }

        /// <summary>
        /// Retorna la fecha de actualización del dominio
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>retorna la fecha de actualizacion</returns>
        public static string GetUpdateDate(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Updated Date:", "\r\n");

            if (values != null && values.Count > 0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }

        /// <summary>
        /// Retorna la fecha de creacion del dominio
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>retorna la fecha de creacion</returns>
        public static string GetCreationDate(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Creation Date:", "\r\n");

            if (values != null && values.Count > 0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }

        /// <summary>
        /// Retorna la fecha de expiracion del dominio
        /// </summary>
        /// <param name="whoisServer">whois server</param>
        /// <param name="url">url</param>
        /// <returns>retorna la fecha de expiracion</returns>
        public static string GetExpirationDate(string whoisServer, string url)
        {
            ValidationHelper.NotBlank(whoisServer);
            ValidationHelper.NotBlank(url);

            var values = GetValueByProperty(whoisServer, url, "Expiration Date:", "\r\n");

            if (values != null && values.Count > 0)
                return values[0].Trim(IgnoreCharacters);

            return string.Empty;
        }
    }
}
