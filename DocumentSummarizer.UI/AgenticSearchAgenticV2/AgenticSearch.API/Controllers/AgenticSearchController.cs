[ApiController]
[Route("api/[controller]")]
public class AgenticSearchController : ControllerBase
{
    private readonly OpenAIAgentService _agent;

    public AgenticSearchController(OpenAIAgentService agent)
    {
        _agent = agent;
    }

    [HttpPost("query")]
    public async Task<IActionResult> Query([FromBody] ToolFunctionRequest request)
    {
        var result = await _agent.HandleQueryAsync(request.Query);
        return Ok(result);
    }
}