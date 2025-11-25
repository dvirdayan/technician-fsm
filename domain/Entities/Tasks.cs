public class task
{
    public string Location { get; set; }
    public string[] RequiredSkills { get; set; }
    public string Duration { get; set; }
    public string TimeWindow { get; set; }
    public task(string location, string[] requiredSkill, string Duration, string TimeWindow)
    {
        this.Location = location;
        this.RequiredSkill = requiredSkill;
        this.Duration = Duration;
        this.TimeWindow = TimeWindow;
    }
}