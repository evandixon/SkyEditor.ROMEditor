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
    <Global.System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0"),  _
     Global.System.Diagnostics.DebuggerNonUserCodeAttribute(),  _
     Global.System.Runtime.CompilerServices.CompilerGeneratedAttribute(),  _
     Global.Microsoft.VisualBasic.HideModuleNameAttribute()>  _
    Friend Module Resources
        
        Private resourceMan As Global.System.Resources.ResourceManager
        
        Private resourceCulture As Global.System.Globalization.CultureInfo
        
        '''<summary>
        '''  Returns the cached ResourceManager instance used by this class.
        '''</summary>
        <Global.System.ComponentModel.EditorBrowsableAttribute(Global.System.ComponentModel.EditorBrowsableState.Advanced)>  _
        Friend ReadOnly Property ResourceManager() As Global.System.Resources.ResourceManager
            Get
                If Object.ReferenceEquals(resourceMan, Nothing) Then
                    Dim temp As Global.System.Resources.ResourceManager = New Global.System.Resources.ResourceManager("SkyEditor.ROMEditor.Resources", GetType(Resources).Assembly)
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
        Friend Property Culture() As Global.System.Globalization.CultureInfo
            Get
                Return resourceCulture
            End Get
            Set
                resourceCulture = value
            End Set
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Byte[].
        '''</summary>
        Friend ReadOnly Property ctrtool() As Byte()
            Get
                Dim obj As Object = ResourceManager.GetObject("ctrtool", resourceCulture)
                Return CType(obj,Byte())
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;PMD2&gt;
        '''  &lt;!--=======================================================================--&gt;
        '''  &lt;!--PPMDU Configuration File--&gt;
        '''  &lt;!--=======================================================================--&gt;
        '''  &lt;!--This file is meant to contain all the data that the program uses --&gt;
        '''  &lt;!--at runtime that could be considered version specific, or that might--&gt;
        '''  &lt;!--change at one point.--&gt;
        '''  
        '''  &lt;!--Layout:--&gt;
        '''  &lt;!--Its made of the following structure this far : --&gt;
        '''  &lt;!--&lt;PMD2&gt;--&gt;
        '''  &lt;!--  &lt;GameEditions / [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property pmd2data() As String
            Get
                Return ResourceManager.GetString("pmd2data", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to &lt;PMD2&gt;
        '''  &lt;!-- Contains reference data from the game --&gt;
        '''  &lt;ScriptData&gt;
        '''    
        '''    &lt;!--Common to all 3--&gt;
        '''    &lt;Game id=&quot;EoS_NA&quot; id2=&quot;EoS_EU&quot; id3=&quot;EoS_JP&quot;&gt;
        '''    
        '''      &lt;!--**********************************************--&gt;
        '''      &lt;!--Game Variables Data--&gt;
        '''      &lt;!--**********************************************--&gt;
        '''      &lt;GameVariablesTable&gt;
        '''        &lt;GameVar type=&quot;8&quot; unk1=&quot;2&quot; memoffset=&quot;  0x0&quot; bitshift=&quot;0x0&quot; unk3=&quot;  0x1&quot; unk4=&quot;0x1&quot; name=&quot;VERSION&quot; /&gt;
        '''        &lt;GameVar type=&quot;8&quot; unk1=&quot;2&quot; memoffset=&quot;  0x4&quot;  [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property pmd2scriptdata() As String
            Get
                Return ResourceManager.GetString("pmd2scriptdata", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Byte[].
        '''</summary>
        Friend ReadOnly Property ppmd_pxcomp() As Byte()
            Get
                Dim obj As Object = ResourceManager.GetObject("ppmd_pxcomp", resourceCulture)
                Return CType(obj,Byte())
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Byte[].
        '''</summary>
        Friend ReadOnly Property ppmd_statsutil() As Byte()
            Get
                Dim obj As Object = ResourceManager.GetObject("ppmd_statsutil", resourceCulture)
                Return CType(obj,Byte())
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Byte[].
        '''</summary>
        Friend ReadOnly Property ppmd_unpx() As Byte()
            Get
                Dim obj As Object = ResourceManager.GetObject("ppmd_unpx", resourceCulture)
                Return CType(obj,Byte())
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized string similar to dofile(&quot;script/include/inc_all.lua&quot;)
        '''dofile(&quot;script/include/inc_event.lua&quot;)
        '''dofile(&quot;script/include/inc_charchoice.lua&quot;)
        '''local sunny = 0
        '''local calm = 0
        '''local flexible = 0
        '''local serious = 0
        '''local cooperate = 0
        '''local chartype = &quot;&quot;
        '''local basecharname = &quot;&quot;
        '''local charname = &quot;&quot;
        '''local persontype = &quot;&quot;
        '''local pertnername = &quot;&quot;
        '''local pertnergender = &quot;&quot;
        '''function groundInit()
        '''end
        '''function groundStart()
        '''end
        '''function seikakushindansetsumei01_init()
        '''end
        '''function seikakushindansetsumei01_start()
        '''  CAMERA: [rest of string was truncated]&quot;;.
        '''</summary>
        Friend ReadOnly Property PSMDStarterIntroScript() As String
            Get
                Return ResourceManager.GetString("PSMDStarterIntroScript", resourceCulture)
            End Get
        End Property
        
        '''<summary>
        '''  Looks up a localized resource of type System.Byte[].
        '''</summary>
        Friend ReadOnly Property vgmstream() As Byte()
            Get
                Dim obj As Object = ResourceManager.GetObject("vgmstream", resourceCulture)
                Return CType(obj,Byte())
            End Get
        End Property
    End Module
End Namespace