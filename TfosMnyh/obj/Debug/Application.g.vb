﻿#ExternalChecksum("..\..\Application.xaml","{406ea660-64cf-4c82-b6f0-42d48172a799}","E79FF07AD5FD8D7F9CB8DAE3B184BAA6")
'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.1
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict Off
Option Explicit On

Imports System
Imports System.Diagnostics
Imports System.Windows
Imports System.Windows.Automation
Imports System.Windows.Controls
Imports System.Windows.Controls.Primitives
Imports System.Windows.Data
Imports System.Windows.Documents
Imports System.Windows.Ink
Imports System.Windows.Input
Imports System.Windows.Markup
Imports System.Windows.Media
Imports System.Windows.Media.Animation
Imports System.Windows.Media.Effects
Imports System.Windows.Media.Imaging
Imports System.Windows.Media.Media3D
Imports System.Windows.Media.TextFormatting
Imports System.Windows.Navigation
Imports System.Windows.Shapes


'''<summary>
'''Application
'''</summary>
Partial Public Class Application
    Inherits System.Windows.Application

    '''<summary>
    '''InitializeComponent
    '''</summary>
    <System.Diagnostics.DebuggerNonUserCodeAttribute()> _
    Public Sub InitializeComponent()

        #ExternalSource("..\..\Application.xaml",4)
        Me.StartupUri = New System.Uri("Window1.xaml", System.UriKind.Relative)

        #End ExternalSource
    End Sub

    '''<summary>
    '''Application Entry Point.
    '''</summary>
    <System.STAThreadAttribute(), _
     System.Diagnostics.DebuggerNonUserCodeAttribute()> _
    Public Shared Sub Main()
        Dim app As Application = New Application()
        app.InitializeComponent()
        app.Run()
    End Sub
End Class
