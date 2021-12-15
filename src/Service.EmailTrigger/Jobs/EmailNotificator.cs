using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.PasswordRecovery.Domain.Models;
using Service.Registration.Domain.Models;

namespace Service.EmailTrigger.Jobs
{
	public class EmailNotificator
	{
		private readonly IEmailSenderService _emailSender;
		private readonly ILogger<EmailNotificator> _logger;

		public EmailNotificator(ILogger<EmailNotificator> logger, IEmailSenderService emailSender,
			ISubscriber<IReadOnlyList<RecoveryInfoServiceBusModel>> recoverySubscriber,
			ISubscriber<IReadOnlyList<RegistrationInfoServiceBusModel>> confirmSubscriber)
		{
			_logger = logger;
			_emailSender = emailSender;
			recoverySubscriber.Subscribe(HandleRecoveryEvent);
			confirmSubscriber.Subscribe(HandleConfirmEvent);
		}

		private async ValueTask HandleRecoveryEvent(IReadOnlyList<RecoveryInfoServiceBusModel> events)
		{
			foreach (RecoveryInfoServiceBusModel message in events)
			{
				string email = message.Email;

				_logger.LogInformation("Sending RecoveryPasswordEmail to user {email}", email);

				await _emailSender.SendRecoveryPasswordEmailAsync(new RecoveryInfoGrpcRequest
				{
					Email = email,
					Hash = message.Hash
				});
			}
		}

		private async ValueTask HandleConfirmEvent(IReadOnlyList<RegistrationInfoServiceBusModel> events)
		{
			foreach (RegistrationInfoServiceBusModel message in events)
			{
				string email = message.Email;

				_logger.LogInformation("Sending RegistrationConfirmEmail to user {email}", email);

				await _emailSender.SendRegistrationConfirmEmailAsync(new RegistrationConfirmGrpcRequest
				{
					Hash = message.Hash
				});
			}
		}
	}
}