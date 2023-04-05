using System.ComponentModel.DataAnnotations;

namespace Althea.Models;

public class AddSystemMessageRequestDto
{
    /// <summary>
    ///     设定信息
    /// </summary>
    [Required]
    public string SystemMessage { get; set; } = null!;
}
