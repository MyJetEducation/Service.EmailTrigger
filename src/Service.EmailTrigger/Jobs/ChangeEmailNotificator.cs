using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.ServiceBus.Models;

namespace Service.EmailTrigger.Jobs
{
	public class ChangeEmailNotificator
	{
		private readonly IEmailSenderService _emailSender;
		private readonly ILogger<ChangeEmailNotificator> _logger;

		public ChangeEmailNotificator(ILogger<ChangeEmailNotificator> logger, IEmailSenderService emailSender,
			ISubscriber<IReadOnlyList<ChangeEmailServiceBusModel>> subscriber)
		{
			_logger = logger;
			_emailSender = emailSender;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<ChangeEmailServiceBusModel> events)
		{
			foreach (ChangeEmailServiceBusModel message in events)
			{
				string email = message.Email;

				_logger.LogInformation("Sending ChangeEmailGrpcRequest for user: {email}", email);

				await _emailSender.SendChangeEmailAsync(new ChangeEmailGrpcRequest
				{
					Email = email,
					Hash = message.Hash
				});
			}
		}
	}
}