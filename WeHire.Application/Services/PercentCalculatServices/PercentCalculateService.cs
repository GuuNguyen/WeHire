using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using WeHire.Application.DTOs.Developer;
using WeHire.Domain.Entities;

namespace WeHire.Application.Services.PercentCalculatServices
{
    public class PercentCalculateService : IPercentCalculateService
    {
        public MatchingPercentage CalculateMatchingPercentage(HiringRequest request, Developer developer)
        {
            double matchPercentage = 0;

            double skillPercentage = CalculateSkillCompatibility(developer, request);
            double levelPercentage = CalculateLevelCompatibility(developer, request);
            double typePercentage = CalculateTypeCompatibility(developer, request);
            double salaryPercentage = CalculateSalaryCompatibility((decimal)request.SalaryPerDev!, (decimal)developer.AverageSalary!);

            matchPercentage = (salaryPercentage + skillPercentage + levelPercentage + typePercentage) / 4;

            var matchingPercentObj = new MatchingPercentage
            {
                TypeMatching = typePercentage.Equals(100),
                LevelMatching = levelPercentage.Equals(100),
                SalaryPerDevPercentage = Math.Round(salaryPercentage, 0),
                SkillPercentage = Math.Round(skillPercentage, 0),
                AveragedPercentage = Math.Round(matchPercentage, 0)
            };

            return matchingPercentObj;
        }

        private double CalculateSkillCompatibility(Developer developer, HiringRequest request)
        {
            var skillRequireIds = request.SkillRequires.Select(s => s.SkillId).ToList();
            var developerSkillIds = developer.DeveloperSkills.Select(s => s.SkillId).ToList();
            double skillPercentage = 0;
            if (skillRequireIds.Count > 0)
            {
                int matchingSkills = developerSkillIds.Intersect(skillRequireIds).Count();
                skillPercentage = (double)matchingSkills / skillRequireIds.Count * 100;
            }
            return skillPercentage;
        }

        private double CalculateLevelCompatibility(Developer developer, HiringRequest request)
        {
            double levelPercentage = (request.LevelRequireId == developer.LevelId) ? 100 : 0;
            return levelPercentage;
        }

        private double CalculateTypeCompatibility(Developer developer, HiringRequest request)
        {
            double typePercentage = developer.DeveloperTypes.Any(t => t.TypeId == request.TypeRequireId) ? 100 : 0;
            return typePercentage;
        }


        private double CalculateSalaryCompatibility(decimal requiredSalary, decimal developerSalary)
        {
            if (requiredSalary == developerSalary)
            {
                return 100;
            }

            decimal minSalary = Math.Min(requiredSalary, developerSalary);
            decimal maxSalary = Math.Max(requiredSalary, developerSalary);

            double percentDifference = (double)(minSalary / maxSalary) * 100;
            return percentDifference;
        }

    }
}
