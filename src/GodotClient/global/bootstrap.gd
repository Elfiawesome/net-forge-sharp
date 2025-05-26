extends Node

var CBootstrapper := preload("res://global/Bootstrapper.cs")

func _ready() -> void:
	# Setup c# Initializer bootstrap thingy!
	CBootstrapper.Initialize()
