﻿'------------------------------------------------------------------------------
' <auto-generated>
'     This code was generated by a tool.
'     Runtime Version:4.0.30319.42000
'
'     Changes to this file may cause incorrect behavior and will be lost if
'     the code is regenerated.
' </auto-generated>
'------------------------------------------------------------------------------

Option Strict On
Option Explicit On

Imports System

Namespace My.Resources
    
    'This class was auto-generated by the StronglyTypedResourceBuilder
    'class via a tool like ResGen or Visual Studio.
    'To add or remove a member, edit your .ResX file then rerun ResGen
    'with the /str option, or rebuild your VS project.
    '''<summary>
    '''  A strongly-typed resource class, for looking up localized strings, etc.
    '''</summary>
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "16.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute()>  _
    Public Class Language
        
        Private Shared resourceMan As Global.System.Resources.ResourceManager
        
        Private Shared resourceCulture As Global.System.Globalization.CultureInfo
        
        <Global.System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")>  _
        Friend Sub New()
            MyBase.New
        End Sub
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("SkyEditor.ROMEditor.Language", GetType(Language).Assembly)
                    resourceMan = temp
                End If
                Return resourceMan
            End Get
        End Property
        
        '''<summary>
        '''  Overrides the current thread's CurrentUICulture property for all
        '''  resource lookups using this strongly typed resource class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Public Shared Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to BGRS File.
        '''</summary>
        Public Shared ReadOnly Property FileType_Bgrs() As String
            Get
                Return ResourceManager.GetString("FileType_Bgrs", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Binary File.
        '''</summary>
        Public Shared ReadOnly Property FileType_Bin() As String
            Get
                Return ResourceManager.GetString("FileType_Bin", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to (None).
        '''</summary>
        Public Shared ReadOnly Property NonePokemon() As String
            Get
                Return ResourceManager.GetString("NonePokemon", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to evandixon.
        '''</summary>
        Public Shared ReadOnly Property PluginAuthor() As String
            Get
                Return ResourceManager.GetString("PluginAuthor", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to Rom Editor Credits:
        '''psy_commando (Pokemon portraits, most of the research)
        '''Grovyle91 (Language strings)
        '''evandixon (Personality test, bgp files).
        '''</summary>
        Public Shared ReadOnly Property PluginCredits() As String
            Get
                Return ResourceManager.GetString("PluginCredits", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to ROM Editor.
        '''</summary>
        Public Shared ReadOnly Property PluginName() As String
            Get
                Return ResourceManager.GetString("PluginName", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to The program &quot;{0}&quot; exited with an unsuccessful exit code: {1}..
        '''</summary>
        Public Shared ReadOnly Property ProcessManagement_UnsuccessfulExitCodeExceptionMessage() As String
            Get
                Return ResourceManager.GetString("ProcessManagement_UnsuccessfulExitCodeExceptionMessage", resourceCulture)
            End Get
        End Property
    End Class
End Namespace
