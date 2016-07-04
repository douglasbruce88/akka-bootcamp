﻿namespace ChartApp

open Akka.Actor
open Akka.FSharp
open System.Drawing
open System.Windows.Forms
open System.Windows.Forms.DataVisualization.Charting

[<AutoOpen>]
module Form = 
    let sysChart = new Chart(Name = "sysChart", Text = "sysChart", Dock = DockStyle.Fill, Location = Point(0, 0), Size = Size(684, 446), TabIndex = 0)
    let form = new Form(Name = "Main", Visible = true, Text = "System Metrics", AutoScaleDimensions = SizeF(6.F, 13.F), AutoScaleMode = AutoScaleMode.Font, ClientSize = Size(684, 446))
    let chartArea1 = new ChartArea(Name = "ChartArea1")
    let legend1 = new Legend(Name = "Legend1")
    let series1 = new Series(Name = "Series1", ChartArea = "ChartArea1", Legend = "Legend1")
    let btnCpu = new Button(Name = "btnCpu", Text = "CPU (ON)", Location = Point(562, 274), Size = Size(110, 41), TabIndex = 1, UseVisualStyleBackColor = true)
    let btnMemory = new Button(Name = "btnMemory", Text = "MEMORY (OFF)", Location = Point(562, 321), Size = Size(110, 41), TabIndex = 2, UseVisualStyleBackColor = true)
    let btnDisk = new Button(Name = "btnDisk", Text = "DISK (OFF)", Location = Point(562, 368), Size = Size(110, 41), TabIndex = 3, UseVisualStyleBackColor = true)
    let btnPauseResume = new Button(Name = "btnPauseResume", Text = "PAUSE ||", Location = Point(562, 205), Size = Size(110, 41), TabIndex = 3, UseVisualStyleBackColor = true)
    
    sysChart.BeginInit()
    form.SuspendLayout()
    sysChart.ChartAreas.Add chartArea1
    sysChart.Legends.Add legend1
    form.Controls.Add btnCpu
    form.Controls.Add btnMemory
    form.Controls.Add btnDisk    
    form.Controls.Add btnPauseResume
    form.Controls.Add sysChart
    sysChart.EndInit()
    form.ResumeLayout false
    
    let load (myActorSystem : ActorSystem) = 
        let chartActor = spawn myActorSystem "charting" (Actors.chartingActor sysChart btnPauseResume)
        let coordinatorActor = spawn myActorSystem "counters" (Actors.performanceCounterCoordinatorActor chartActor)
        
        let toggleActors = 
            Map.ofList 
                [ (CounterType.Cpu, 
                   spawnOpt myActorSystem "cpuCounter" (Actors.buttonToggleActor coordinatorActor btnCpu CounterType.Cpu false) [ SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher") ]) // CPU Toggle Actor
                  
                  (CounterType.Memory, 
                   spawnOpt myActorSystem "memoryCounter" (Actors.buttonToggleActor coordinatorActor btnMemory CounterType.Memory false) 
                       [ SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher") ]) // Memory Toggle Actor
                  
                  (CounterType.Disk, 
                   spawnOpt myActorSystem "diskCounter" (Actors.buttonToggleActor coordinatorActor btnDisk CounterType.Disk false) [ SpawnOption.Dispatcher("akka.actor.synchronized-dispatcher") ]) // Disk Toggle Actor
                                                                                                                                                                                                     ]
        toggleActors.[CounterType.Cpu] <! Toggle
        btnCpu.Click.Add(fun _ -> toggleActors.[CounterType.Cpu] <! Toggle)
        btnMemory.Click.Add(fun _ -> toggleActors.[CounterType.Memory] <! Toggle)
        btnDisk.Click.Add(fun _ -> toggleActors.[CounterType.Disk] <! Toggle)
        btnPauseResume.Click.Add (fun _ -> chartActor <! TogglePause)
        form
