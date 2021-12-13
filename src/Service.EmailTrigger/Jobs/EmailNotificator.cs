using System.Collections.Generic;
using System.Threading.Tasks;
using DotNetCoreDecorators;
using Microsoft.Extensions.Logging;
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
			var taskList = new List<Task>();

			foreach (RecoveryInfoServiceBusModel message in events)
			{
				string email = message.Email;

				Task<CommonGrpcResponse> task = _emailSender.SendRecoveryPasswordEmailAsync(new RecoveryInfoGrpcRequest
				{
					Email = email,
					Hash = message.Hash
				}).AsTask();

				taskList.Add(task);

				_logger.LogInformation("Sending RecoveryPasswordEmail to user {email}", email);
			}
			await Task.WhenAll(taskList);
		}
	}
}