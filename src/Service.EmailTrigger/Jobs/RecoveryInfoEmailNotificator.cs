using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.ServiceBus.Models;

namespace Service.EmailTrigger.Jobs
{
	public class RecoveryInfoEmailNotificator
	{
		private readonly IEmailSenderService _emailSender;
		private readonly ILogger<RecoveryInfoEmailNotificator> _logger;

		public RecoveryInfoEmailNotificator(ILogger<RecoveryInfoEmailNotificator> logger, IEmailSenderService emailSender,
			ISubscriber<IReadOnlyList<RecoveryInfoServiceBusModel>> subscriber)
		{
			_logger = logger;
			_emailSender = emailSender;
			subscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<RecoveryInfoServiceBusModel> events)
		{
			foreach (RecoveryInfoServiceBusModel message in events)
			{
				string email = message.Email;

				_logger.LogInformation("Sending RecoveryInfoGrpcRequest for user: {email}", email);

				await _emailSender.SendRecoveryPasswordEmailAsync(new RecoveryInfoGrpcRequest
				{
					Email = email,
					Hash = message.Hash
				});
			}
		}
	}
}