// Learn more about F# at http://docs.microsoft.com/dotnet/fsharp
module Program

open System
open System.Net
open System.Threading.Tasks
open Microsoft.Extensions.Hosting
open Microsoft.Azure.Functions.Worker
open Microsoft.Azure.Functions.Worker.Http


[<Function("FunctionNoParams")>]
let FunctionNoParams
    ([<HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "function-no-params")>] req: HttpRequestData,
      execCtxt: FunctionContext): Task<_> =

  async {
    let response = req.CreateResponse(HttpStatusCode.OK)
    do! (response.WriteAsJsonAsync {| intField = 10; stringField = "Hello World!"|}).AsTask() |> Async.AwaitTask
    return response
  }
  |> Async.StartAsTask



[<Function("FunctionWithParams")>]
let FunctionWithParams
    ([<HttpTrigger(AuthorizationLevel.Anonymous, "GET", Route = "function-with-params/{a:int}/{b:int}")>] req: HttpRequestData,
      a: int, b: int, execCtxt: FunctionContext): Task<_> =

  async {
    let response = req.CreateResponse(HttpStatusCode.OK);
    response.Headers.Add("Content-Type", "text/plain")

    do! response.WriteStringAsync($"a: {a}, b: {b}") |> Async.AwaitTask
    return response
  }
  |> Async.StartAsTask



[<EntryPoint>]
let main argv =
  async {
    do! Async.SwitchToThreadPool ()

    let host =
      (new HostBuilder())
        .ConfigureFunctionsWorkerDefaults()
        .Build()

    return! host.RunAsync() |> Async.AwaitTask
  }
  |> Async.RunSynchronously

  0
