class_name Client extends Node2D

signal button_pressed


var scene = preload("res://server_bridge/ClientNode.cs")
var client = scene.new()

func _ready() -> void:
	client.ConnectToServer()
	$CanvasLayer/Control/VBoxContainer/ServerShutdown.pressed.connect(_button1_pressed)
	$CanvasLayer/Control/VBoxContainer/ClientDisconnect.pressed.connect(_button2_pressed)
	$CanvasLayer/Control/VBoxContainer/DataTest.pressed.connect(_button3_pressed)

func _button1_pressed(): button_pressed.emit()
func _button2_pressed(): client.LeaveServer()
func _button3_pressed(): client.SendData("Hello world")
