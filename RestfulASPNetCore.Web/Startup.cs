using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Formatters;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using RestfulASPNetCore.Web.Dtos;
using RestfulASPNetCore.Web.Entities;
using RestfulASPNetCore.Web.Helpers;
using RestfulASPNetCore.Web.Services;

namespace RestfulASPNetCore.Web
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
            services.AddMvc(
                setup =>
                {
                    setup.ReturnHttpNotAcceptable = true;
                    setup.OutputFormatters.Add(new XmlDataContractSerializerOutputFormatter());
                }
            )
            .SetCompatibilityVersion(CompatibilityVersion.Version_2_1);
            var connectionString = Configuration["connectionStrings:libraryDBConnectionString"];
            services.AddDbContext<LibraryContext>(o => o.UseSqlServer(connectionString));
            services.AddScoped<ILibraryRepository, LibraryRepository>();
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, LibraryContext libraryContext)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }
            else
            {
                app.UseHsts();
            }

            app.UseHttpsRedirection();


            AutoMapper.Mapper.Initialize(cfg =>
            {
                cfg.CreateMap<Entities.Author, Dtos.Author>()
                    .ForMember(dest => dest.Name, opt => opt.MapFrom(src => $"{src.FirstName} {src.LastName}"))
                    .ForMember(dest => dest.Age, opt => opt.MapFrom(src =>
                        src.DateOfBirth.GetCurrentAge()));

                cfg.CreateMap<Entities.Book, Dtos.Book>();

                cfg.CreateMap<CreateAuthor, Entities.Author>();
            });

            libraryContext.EnsureSeedDataForContext();

            app.UseMvc();
        }
    }
}
