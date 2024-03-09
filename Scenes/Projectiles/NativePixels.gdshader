shader_type canvas_item;

uniform vec4 tint_color : source_color = vec4(1.0);

void vertex() {
}

void fragment() {
	// Called for every pixel the material is visible on.
	COLOR = texture(TEXTURE, UV);
	
	float grayscale = (COLOR.a * COLOR.g * COLOR.b);
	COLOR = tint_color * vec4(grayscale, grayscale, grayscale, COLOR.a);
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
