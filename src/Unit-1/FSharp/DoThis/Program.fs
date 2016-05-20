open Akka.FSharp
open Akka.FSharp.Spawn
open WinTail

[<EntryPoint>]
let main argv = 
    // initialize an actor system
    let myActorSystem = System.create "MyActorSystem" (Configuration.load())

    // make your first actors using the 'spawn' function
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)
    let consoleReaderActor = spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor consoleWriterActor))

    // tell the consoleReader actor to begin
    consoleReaderActor <! Start
    myActorSystem.AwaitTermination()
    0
