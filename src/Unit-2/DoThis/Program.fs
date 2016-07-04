open Akka.FSharp
open ChartApp
open System
open System.Windows.Forms

let chartActors = System.create "ChartActors" (Configuration.load())

Application.EnableVisualStyles()
Application.SetCompatibleTextRenderingDefault false
[<STAThread>]
do Application.Run(Form.load chartActors)
