using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.ServiceBus.Models;

namespace Service.EmailTrigger.Jobs
{
	public class RegistrationInfoEmailNotificator
	{
		private readonly IEmailSenderService _emailSender;
		private readonly ILogger<RegistrationInfoEmailNotificator> _logger;

		public RegistrationInfoEmailNotificator(ILogger<RegistrationInfoEmailNotificator> logger, IEmailSenderService emailSender,
			ISubscriber<IReadOnlyList<RegistrationInfoServiceBusModel>> subscriber)
		{
			_logger = logger;
			_emailSender = emailSender;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<RegistrationInfoServiceBusModel> events)
		{
			foreach (RegistrationInfoServiceBusModel message in events)
			{
				string email = message.Email;

				_logger.LogInformation("Sending RegistrationConfirmGrpcRequest for user: {email}", email);

				await _emailSender.SendRegistrationConfirmEmailAsync(new RegistrationConfirmGrpcRequest
				{
					Email = email,
					Hash = message.Hash
				});
			}
		}
	}
}