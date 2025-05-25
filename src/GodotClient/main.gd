extends Node

@onready var server_node := $ServerNode
@onready var client_node := $ClientNode

func _ready() -> void:
	server_node.Start()
	client_node.ConnectToServer()

func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed:
			if event.keycode == KEY_ESCAPE:
				server_node.Shutdown()
	
