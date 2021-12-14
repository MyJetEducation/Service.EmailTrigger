using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
using Service.Core.Grpc.Models;
using Service.EmailSender.Grpc;
using Service.EmailSender.Grpc.Models;
using Service.PasswordRecovery.Domain.Models;

namespace Service.EmailTrigger.Jobs
{
	public class EmailNotificator
	{
		private readonly IEmailSenderService _emailSender;
		private readonly ILogger<EmailNotificator> _logger;

		public EmailNotificator(ILogger<EmailNotificator> logger, ISubscriber<IReadOnlyList<RecoveryInfoServiceBusModel>> registerSubscriber, IEmailSenderService emailSender)
		{
			_logger = logger;
			_emailSender = emailSender;
			registerSubscriber.Subscribe(HandleEvent);
		}

		private async ValueTask HandleEvent(IReadOnlyList<RecoveryInfoServiceBusModel> events)
		{
			var taskList = new List<ValueTask<CommonGrpcResponse>>();

			foreach (RecoveryInfoServiceBusModel message in events)
			{
				string email = message.Email;

				taskList.Add(_emailSender.SendRecoveryPasswordEmailAsync(new RecoveryInfoGrpcRequest
				{
					Email = email,
					Hash = message.Hash
				}));

				_logger.LogInformation("Sending RecoveryPasswordEmail to user {email}", email);
			}

			await Task.WhenAll(
				taskList
					.Where(task => !task.IsCompletedSuccessfully)
					.Select(task => task.AsTask())
				);
		}
	}
}