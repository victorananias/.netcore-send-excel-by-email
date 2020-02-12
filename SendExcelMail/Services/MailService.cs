﻿using System;
using System.Net;
using System.Net.Mail;
using Microsoft.Extensions.Options;
using SendExcelMail.Settings;

namespace SendExcelMail.Services
{
    public class MailService
    {
        private string _host;
        private int _port;
        private string _cc;
        private string _bcc;
        private string _to;
        private MailAddress _from;
        private NetworkCredential _credentials;

        public MailService(IOptions<AppSettings> appSettings)
        {
            var settings = appSettings.Value.MailSettings;
            _host = settings.Host;
            _port = settings.Port;
            _from = new MailAddress(string.IsNullOrEmpty(settings.From) ? settings.Username : settings.From);
            _credentials = new NetworkCredential(settings.Username, settings.Password);
        }

        public MailService From(string from, string fromName = null)
        {
            _from = new MailAddress(from, fromName);
            return this;
        }

        public MailService CC(string cc)
        {
            _cc = cc;
            return this;
        }

        public MailService BCC(string bcc)
        {
            _bcc = bcc;
            return this;
        }

        public MailService To(string to)
        {
            _to = to;
            return this;
        }

        public void Send(string subject, string body, bool isBodyHtml = true)
        {
            using var client = new SmtpClient(_host, _port)
            {
                EnableSsl = true,
                Credentials = _credentials
            };

            var message = new MailMessage
            {
                From = _from,
                Subject = subject,
                Body = body,
                IsBodyHtml = isBodyHtml
            };

            AddAdress(message.To, _to);
            AddAdress(message.CC, _cc);
            AddAdress(message.Bcc, _bcc);

            client.Send(message);
        }

        private void AddAdress(MailAddressCollection mail, string adresses)
        {
            if (string.IsNullOrEmpty(adresses))
            {
                return;
            }

            foreach (var address in adresses.Split(new[] {";"}, StringSplitOptions.RemoveEmptyEntries))
            {
                mail.Add(address);
            }
        }
    }
}