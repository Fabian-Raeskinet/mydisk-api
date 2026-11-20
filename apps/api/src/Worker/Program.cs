using MyDisks.DependencyInjection.Configurations;
using MyDisks.Messaging;
using MyDisks.Services.Reviews.Moderation;

var builder = Host.CreateApplicationBuilder(args);

builder.Services.RegisterApplicationServices(builder.Configuration);
builder.Services.Configure<ReviewModerationSettings>(builder.Configuration.GetSection(ReviewModerationSettings.SectionName));
builder.Services.AddRabbitMqMessaging(builder.Configuration);

builder.Build().Run();
