using AutoMapper;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.Utilities.Helper.Mapping.MappingProfile;

namespace WeHire.Application.Utilities.Helper.Mapping
{
    public static class MappingConfig
    {
        public static void ConfigureAutoMapper(this IServiceCollection services)
        {
            var mappingConfig = new MapperConfiguration(mc =>
            {
                mc.AddProfile(new UserProfile());
                mc.AddProfile(new DeveloperProfile());
                mc.AddProfile(new SkillProfile());
                mc.AddProfile(new TypeProfile());
                mc.AddProfile(new LevelProfile());
                mc.AddProfile(new CompanyProfile());
                mc.AddProfile(new HiringRequestProfile());
                mc.AddProfile(new RoleProfile());
                mc.AddProfile(new GenderProfile());
                mc.AddProfile(new EmploymentTypeProfile());
                mc.AddProfile(new TransactionProfile());
                mc.AddProfile(new InterviewProfile());
                mc.AddProfile(new EducationProfile());
                mc.AddProfile(new ProfessionalExperienceProfile());
                mc.AddProfile(new NotificationProfile());
                mc.AddProfile(new UserDeviceProfile());
                mc.AddProfile(new ProjectProfile());
                mc.AddProfile(new ProjectTypeProfile());
                mc.AddProfile(new ContractProfile());
                mc.AddProfile(new PayPeriodProfile());
                mc.AddProfile(new PaySlipProfile());
                mc.AddProfile(new WorkLogProfile());
                mc.AddProfile(new HiredDeveloperProfile());
                mc.AddProfile(new ReportProfile());
                mc.AddProfile(new ReportTypeProfile());
            });

            IMapper mapper = mappingConfig.CreateMapper();
            services.AddSingleton(mapper);
        }
    }
}
