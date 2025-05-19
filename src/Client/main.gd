extends Node

@onready var server_node := $ServerNode

func _input(event: InputEvent) -> void:
	if event is InputEventKey:
		if event.pressed and event.keycode == KEY_ESCAPE:
			# Shutdown server
			print("Sending shutdown signal...")
			server_node.Shutdown()
