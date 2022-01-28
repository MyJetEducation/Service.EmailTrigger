using Autofac;
using MyServiceBus.TcpClient;
using Service.EmailSender.Client;
using Service.PasswordRecovery.Domain.Models;
using Service.Registration.Domain.Models;
using Service.ServiceBus.Services;

namespace Service.EmailTrigger.Modules
{
	public class ServiceModule : Module
	{
		private const string QueueName = "MyJetEducation-EmailTrigger";

		protected override void Load(ContainerBuilder builder)
		{
			builder.RegisterEmailSenderClient(Program.Settings.EmailSenderGrpcServiceUrl);

			MyServiceBusTcpClient serviceBusClient = builder.RegisterServiceBusClient(Program.ReloadedSettings(e => e.ServiceBusReader), Program.LogFactory);
			builder.RegisterServiceBusSubscriberBatch<RecoveryInfoServiceBusModel>(serviceBusClient, RecoveryInfoServiceBusModel.TopicName, QueueName);
			builder.RegisterServiceBusSubscriberBatch<RegistrationInfoServiceBusModel>(serviceBusClient, RegistrationInfoServiceBusModel.TopicName, QueueName);
		}
	}
}