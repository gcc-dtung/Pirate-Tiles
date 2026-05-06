public class ChapterModel
{
    public int CurrentChapterIndex { get; set; }
    public int TotalChapters { get; set; }
    public int LevelsInCurrentChapter { get; set; }
    public int UnlockedLevel { get; set; } // Global unlocked level
    
    public bool CanGoNext => CurrentChapterIndex < TotalChapters - 1;
    public bool CanGoPrev => CurrentChapterIndex > 0;
}
