namespace SalaryApi
{
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

}
