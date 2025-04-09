using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Apps.Braze.Dtos;
public class ErrorOrMessageDto
{
    public List<Error> Errors { get; set; }
    public string Message { get; set; }
}

public class Error
{
    public string Message { get; set; }
}
