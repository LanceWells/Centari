shader_type canvas_item;

uniform vec4 hue : source_color;
uniform float mix_strength;
uniform float opacity: hint_range(0.0, 1.0, 0.1) = 1.0;

void fragment() {
	float time_strength = mix_strength + (1.0 - mix_strength) * sin(TIME * 5.0) * 0.3;
	
	// Called for every pixel the material is visible on.
	COLOR = texture(TEXTURE, UV);
	COLOR.r = mix(COLOR.r, hue.r, time_strength);
	COLOR.g = mix(COLOR.r, hue.g, time_strength);
	COLOR.b = mix(COLOR.r, hue.b, time_strength);
	COLOR.a = COLOR.a * opacity;
}

//void light() {
	// Called for every pixel for every light affecting the CanvasItem.
	// Uncomment to replace the default light processing function with this one.
//}
