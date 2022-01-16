DateTime now = DateTime.Now;
DateTime next = now.AddMinutes(10);

TimeSpan duration = next - now;

Console.WriteLine(duration.Minutes);