using System.ComponentModel.DataAnnotations;

namespace deavnote.model.Enums;

/// <summary>
/// Specifies the possible states of a development task.
/// </summary>
public enum EDevTaskState
{
    [Display(Name = "Unknown state")]
    Unknown = 0,
    [Display(Name = "Not started")]
    NotStarted = 1,
    [Display(Name = "In progress")]
    InProgress = 2,
    [Display(Name = "Completed")]
    Completed = 3,
    [Display(Name = "Merged")]
    Merged = 4,
    [Display(Name = "Tested")]
    Tested = 5,
    [Display(Name = "Rejected")]
    Rejected = 6,
}
