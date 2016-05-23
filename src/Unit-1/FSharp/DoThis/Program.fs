﻿open Akka.Actor
open Akka.FSharp
open Akka.FSharp.Spawn
open System
open WinTail

[<EntryPoint>]
let main _ = 
    let myActorSystem = System.create "MyActorSystem" (Configuration.load())
    let consoleWriterActor = spawn myActorSystem "consoleWriterActor" (actorOf Actors.consoleWriterActor)
    
    //SupervisionStrategy used by tailCoordinatorActor
    let strategy() = 
        Strategy.OneForOne((fun ex -> 
                           match ex with
                           | :? ArithmeticException -> Directive.Resume
                           | :? NotSupportedException -> Directive.Stop
                           | _ -> Directive.Restart), 10, TimeSpan.FromSeconds(30.))
    
    let tailCoordinatorActor = 
        spawnOpt myActorSystem "tailCoordinatorActor" (actorOf2 Actors.tailCoordinatorActor) 
            [ SpawnOption.SupervisorStrategy(strategy()) ]
    let fileValidatorActor = 
        spawn myActorSystem "validationActor" 
            (actorOf2 (Actors.fileValidatorActor consoleWriterActor tailCoordinatorActor))
    let consoleReaderActor = 
        spawn myActorSystem "consoleReaderActor" (actorOf2 (Actors.consoleReaderActor fileValidatorActor))
    consoleReaderActor <! Start
    myActorSystem.AwaitTermination()
    0
