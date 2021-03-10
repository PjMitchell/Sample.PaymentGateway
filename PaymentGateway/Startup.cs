using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.OpenApi.Models;
using PaymentGateway.Auth;
using System.Collections.Generic;

namespace PaymentGateway
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            services.Configure<Dictionary<string, BankConfiguration>>(Configuration.GetSection("Banks"));

            // This should be replaced with an Authentication handler that Authenticates and gets the merchant Id
            services.AddAuthentication(o => {
                o.DefaultScheme = DevAuthenticationHandler.Schema;
            }).AddScheme<DevAuthenticationOptions, DevAuthenticationHandler>(DevAuthenticationHandler.Schema, o => {
                o.MerchantId = "Acme101";
                o.CanReadRequest = true;
                o.CanSubmitRequest = true;
            });

            services.AddAuthorization(options =>
            {
                options.AddPolicy(AuthPolicy.PaymentRead, policy => policy.RequireClaim(MerchantClaims.PaymentRequestClaimType, MerchantClaims.PaymentRequestReadValue));
                options.AddPolicy(AuthPolicy.PaymentSubmission, policy => policy.RequireClaim(MerchantClaims.PaymentRequestClaimType, MerchantClaims.PaymentRequestSubmitValue));
            });

            services.AddTransient<IPaymentRequestProcessor, PaymentRequestProcessor>();
            services.AddTransient<IPaymentRequestValidator, PaymentRequestValidator>();
            services.AddTransient<IBankFactory, BankFactory>();
            services.AddTransient<IBankTransactionService, BankTransactionService>();
            

            services.AddEventBus(Configuration);
            services.AddHostedService<EventListener>();
            services.AddPaymentCollectionStore(Configuration);
            services.AddControllers();
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "PaymentGateway", Version = "v1" });
                c.UseOneOfForPolymorphism();
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "PaymentGateway v1"));
            }
            
            app.UseHttpsRedirection();

            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
    }
}
