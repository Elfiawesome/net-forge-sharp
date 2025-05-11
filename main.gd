extends Node

func _ready() -> void:
	var scene := preload("res://client/client.tscn")
	var client := scene.instantiate()
	add_child(client)
	
	var cscript: CSharpScript = preload("res://server_bridge/ServerNode.cs")
	var server = cscript.new()
	server.Start()
	
	client.button_pressed.connect(
		func()->void:
			server.Stop()
	)
