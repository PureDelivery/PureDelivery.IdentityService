using MailKit.Net.Smtp;
using MailKit.Security;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MimeKit;
using PureDelivery.Common.Configuration.Services;
using PureDelivery.IdentityService.Core.Configuration;
using System;
using System.Threading;
using System.Threading.Tasks;

namespace PureDelivery.IdentityService.Core.Services.impl
{
    public class EmailService : IEmailService
    {
        private readonly ILogger<EmailService> _logger;
        private readonly EmailSettings _emailSettings;

        public EmailService(ILogger<EmailService> logger, ICustomConfigurationProvider configProvider)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _emailSettings = configProvider.GetConfigurationAsync<EmailSettings>("Email").GetAwaiter().GetResult();
        }

        public async Task<bool> SendOtpEmailAsync(string email, string otpCode, CancellationToken cancellationToken = default)
        {
            try
            {
                var subject = "Pure Delivery - Email Confirmation";
                var htmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                            .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                            .header {{ text-align: center; margin-bottom: 30px; }}
                            .otp-code {{ font-size: 32px; font-weight: bold; color: #007bff; text-align: center; padding: 20px; background: #f8f9fa; border-radius: 8px; margin: 20px 0; letter-spacing: 3px; }}
                            .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 14px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1 style='color: #333; margin: 0;'>Welcome to Pure Delivery!</h1>
                            </div>
                            
                            <p>Thank you for creating an account with Pure Delivery. To complete your registration, please confirm your email address by entering this verification code:</p>
                            
                            <div class='otp-code'>{otpCode}</div>
                            
                            <p><strong>Important:</strong></p>
                            <ul>
                                <li>This code will expire in <strong>10 minutes</strong></li>
                                <li>Do not share this code with anyone</li>
                                <li>If you didn't create an account, please ignore this email</li>
                            </ul>
                            
                            <div class='footer'>
                                <p>Best regards,<br>The Pure Delivery Team</p>
                                <p><em>This is an automated message, please do not reply to this email.</em></p>
                            </div>
                        </div>
                    </body>
                    </html>";

                var result = await SendEmailAsync(email, subject, htmlBody, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("OTP email sent successfully to {Email}", email);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send OTP email to {Email}", email);
                return false;
            }
        }

        public async Task<bool> SendWelcomeEmailAsync(string email, string firstName, CancellationToken cancellationToken = default)
        {
            try
            {
                var subject = "Welcome to Pure Delivery!";
                var htmlBody = $@"
                    <!DOCTYPE html>
                    <html>
                    <head>
                        <style>
                            body {{ font-family: Arial, sans-serif; margin: 0; padding: 20px; background-color: #f5f5f5; }}
                            .container {{ max-width: 600px; margin: 0 auto; background: white; padding: 30px; border-radius: 10px; box-shadow: 0 2px 10px rgba(0,0,0,0.1); }}
                            .header {{ text-align: center; margin-bottom: 30px; }}
                            .welcome-message {{ text-align: center; padding: 20px; background: linear-gradient(135deg, #007bff, #0056b3); color: white; border-radius: 8px; margin: 20px 0; }}
                            .footer {{ margin-top: 30px; padding-top: 20px; border-top: 1px solid #eee; color: #666; font-size: 14px; }}
                        </style>
                    </head>
                    <body>
                        <div class='container'>
                            <div class='header'>
                                <h1 style='color: #333; margin: 0;'>🎉 Welcome to Pure Delivery!</h1>
                            </div>
                            
                            <div class='welcome-message'>
                                <h2>Hello {firstName}!</h2>
                                <p>Your email has been successfully confirmed!</p>
                            </div>
                            
                            <p>We're excited to have you as part of the Pure Delivery family. You can now:</p>
                            <ul>
                                <li>Browse our menu and place orders</li>
                                <li>Track your deliveries in real-time</li>
                                <li>Earn loyalty points with every order</li>
                                <li>Save your favorite addresses</li>
                            </ul>
                            
                            <p>Ready to get started? <a href='#' style='color: #007bff; text-decoration: none;'>Place your first order</a> and enjoy fast, reliable delivery!</p>
                            
                            <div class='footer'>
                                <p>Best regards,<br>The Pure Delivery Team</p>
                                <p><em>This is an automated message, please do not reply to this email.</em></p>
                            </div>
                        </div>
                    </body>
                    </html>";

                var result = await SendEmailAsync(email, subject, htmlBody, cancellationToken);

                if (result)
                {
                    _logger.LogInformation("Welcome email sent successfully to {Email}", email);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send welcome email to {Email}", email);
                return false;
            }
        }

        private async Task<bool> SendEmailAsync(string toEmail, string subject, string htmlBody, CancellationToken cancellationToken)
        {
            try
            {
                var message = new MimeMessage();
                message.From.Add(new MailboxAddress(_emailSettings.FromName, _emailSettings.FromEmail));
                message.To.Add(new MailboxAddress("", toEmail));
                message.Subject = subject;

                var bodyBuilder = new BodyBuilder
                {
                    HtmlBody = htmlBody
                };
                message.Body = bodyBuilder.ToMessageBody();

                using var client = new SmtpClient();
                client.CheckCertificateRevocation = false;

                await client.ConnectAsync(_emailSettings.SmtpHost, _emailSettings.SmtpPort,
                    _emailSettings.EnableSsl ? SecureSocketOptions.StartTls : SecureSocketOptions.None,
                    cancellationToken);

                if (!string.IsNullOrEmpty(_emailSettings.SmtpUsername))
                {
                    await client.AuthenticateAsync(_emailSettings.SmtpUsername, _emailSettings.SmtpPassword, cancellationToken);
                }

                await client.SendAsync(message, cancellationToken);
                await client.DisconnectAsync(true, cancellationToken);

                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to send email to {Email}", toEmail);
                return false;
            }
        }
    }
}