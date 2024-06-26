using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;

namespace SalaryApi.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class SalaryController : ControllerBase
    {
        private readonly ILogger<SalaryController> _logger;

        public SalaryController(ILogger<SalaryController> logger)
        {
            _logger = logger;
        }

        [HttpPost("calculate")]
        //חישוב השכר הסופי לאחר העלאה
        public SalaryCalculationResult CalculateSalary(SalaryCalculationRequest request)
        {
            decimal baseSalary = CalculateBaseSalary(request);
            decimal seniorityBonus = CalculateSeniorityBonus(request);
            decimal lawBonus = CalculateLawBonus(request);
            decimal salaryIncrease = CalculateSalaryIncrease(baseSalary, request);
            
            decimal finalSalary = (baseSalary + lawBonus * baseSalary + seniorityBonus * baseSalary) + ((salaryIncrease / 100) * (baseSalary + lawBonus * baseSalary + seniorityBonus * baseSalary));

            var result = new SalaryCalculationResult
            {
                BaseSalary = baseSalary,
                SeniorityBonus = seniorityBonus,
                TotalSeniorityBonusPercentage = seniorityBonus*100,
                TotalSeniorityBonusAmount =seniorityBonus* baseSalary,
                LawBonus = lawBonus,
                TotalLawBonus = lawBonus* baseSalary,
                SalaryBeforeIncrease = baseSalary + lawBonus * baseSalary + seniorityBonus * baseSalary,
                SalaryIncreasePercentage = salaryIncrease,
                TotalSalaryIncrease = (salaryIncrease/100 )* (baseSalary + lawBonus * baseSalary + seniorityBonus * baseSalary),
                FinalSalary = finalSalary
            };

            return result;
        }
        //חישוב שכר הבסיס
        private decimal CalculateBaseSalary(SalaryCalculationRequest request)
        {
            decimal baseHourlyRate = request.ProfessionalLevel switch
            {
                ProfessionalLevel.Beginner => 100,
                ProfessionalLevel.Experienced => 120,
                _ => throw new ArgumentException("Invalid professional level"),
            };

            decimal managerialBonus = request.ManagementLevel switch
            {
                ManagementLevel.None => 0,
                ManagementLevel.Level1 => 20,
                ManagementLevel.Level2 => 40,
                ManagementLevel.Level3 => 60,
                ManagementLevel.Level4 => 80,
                _ => throw new ArgumentException("Invalid management l9evel"),
            };

            decimal fullTimeHours = 170; // Assuming full-time hours per month
            decimal baseSalary = fullTimeHours * (baseHourlyRate + managerialBonus);
            return baseSalary;
        }
       //חישוב תוספת וותק לשכר
        private decimal CalculateSeniorityBonus(SalaryCalculationRequest request)
        {
            decimal seniorityBonusPercentage = 1.25m;
            decimal seniorityBonus = (request.YearsOfExperience * seniorityBonusPercentage)/100 ;
            return seniorityBonus;
        }

        /// חישוב תוספת עבודה מתוקף זכאות בחוק.
        private decimal CalculateLawBonus(SalaryCalculationRequest request)
        {
            decimal lawBonusPercentageGroupA = 0.01m; // 1% bonus
            decimal lawBonusPercentageGroupB = 0.005m; // 0.5% bonus

            decimal lawBonus = request.eligibleForLawAddition== "yes" && request.eligibleForLawAddition == "yes" ?
                (request.GroupMembership == "groupA" ? lawBonusPercentageGroupA : lawBonusPercentageGroupB) :
                0;

            return lawBonus;
        }
        /// חישוב העלאה בשכר על פי שכר הבסיס והתנאים הנוספים.
        
        private decimal CalculateSalaryIncrease(decimal baseSalary, SalaryCalculationRequest request)
        {
            decimal salaryIncreasePercentage;
            if (baseSalary <= 20000)
            {
                salaryIncreasePercentage = 1.5m;
            }
            else if (baseSalary <= 30000)
            {
                salaryIncreasePercentage = 1.25m;
            }
            else
            {
                salaryIncreasePercentage = 1;
            }

            decimal managementIncrease = request.ManagementLevel switch
            {
                ManagementLevel.None => 0,
                ManagementLevel.Level1 => 0.1m,
                ManagementLevel.Level2 => 0.2m,
                ManagementLevel.Level3 => 0.3m,
                ManagementLevel.Level4 => 0.4m,
                _ => throw new ArgumentException("Invalid management level"),
            };


            decimal salaryIncrease =  (salaryIncreasePercentage  + managementIncrease);
            return salaryIncrease;
        }
    }

    public class SalaryCalculationResult
    {
        public decimal BaseSalary { get; set; }
        public decimal SeniorityBonus { get; set; }
        public decimal TotalSeniorityBonusPercentage { get; set; }
        public decimal TotalSeniorityBonusAmount { get; set; }
        public decimal LawBonus { get; set; }
        public decimal TotalLawBonus { get; set; }
        public decimal SalaryBeforeIncrease { get; set; }
        public decimal SalaryIncreasePercentage { get; set; }
        public decimal TotalSalaryIncrease { get; set; }
        public decimal FinalSalary { get; set; }
    }

    public enum ProfessionalLevel
    {
        Beginner,
        Experienced
    }

    public enum ManagementLevel
    {
        None,
        Level1,
        Level2,
        Level3,
        Level4
    }

    
    public class SalaryCalculationRequest
    {
        public ProfessionalLevel? ProfessionalLevel { get; set; }
        public ManagementLevel? ManagementLevel { get; set; }
        public int YearsOfExperience { get; set; }
        public string eligibleForLawAddition { get; set; }
        public string? GroupMembership { get; set; }
        public string? employmentFraction { get; set; }
    }
}
