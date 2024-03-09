shader_type particles;

float rand_from_seed(in uint seed) {
	int k;
	int s = int(seed);
	if (s == 0)
	s = 305420679;
	k = s / 127773;
	s = 16807 * (s - k * 127773) - 2836 * k;
	if (s < 0)
	s += 2147483647;
	seed = uint(s);
	return float(seed % uint(65536)) / 65535.0;
}

uint hash(uint x) {
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = ((x >> uint(16)) ^ x) * uint(73244475);
	x = (x >> uint(16)) ^ x;
	return x;
}

void start() {
	if (RESTART_ROT_SCALE) {
		TRANSFORM[0].xyz = vec3(1.0, 0.0, 0.0);
		TRANSFORM[1].xyz = vec3(0.0, 1.0, 0.0);
		TRANSFORM[2].xyz = vec3(0.0, 0.0, 1.0);
	}
	
	// Called when a particle is spawned.
	uint alt_seed1 = hash(NUMBER + uint(1) + RANDOM_SEED);
	uint alt_seed2 = hash(NUMBER + uint(27) + RANDOM_SEED);
	uint alt_seed3 = hash(NUMBER + uint(43) + RANDOM_SEED);
	uint alt_seed4 = hash(NUMBER + uint(111) + RANDOM_SEED);
	
	CUSTOM.x = rand_from_seed(alt_seed1);
	vec3 position = vec3(rand_from_seed(alt_seed2) * 2.0 - 1.0,
	                     rand_from_seed(alt_seed3) * 2.0 - 1.0,
	                     rand_from_seed(alt_seed4) * 2.0 - 1.0);
	
	VELOCITY.z = 10.0;
	TRANSFORM[3].xyz = position * 20.0;
	
	vec3 scale = vec3(0.1, 0.1, 0.1);
	
	TRANSFORM[0].xyz *= scale.x;
	TRANSFORM[1].xyz *= scale.y;
	TRANSFORM[2].xyz *= scale.z;
}

void process() {
	vec3 scale = vec3(0.1, 0.1, 0.1);
	
	TRANSFORM[0].xyz *= scale.x;
	TRANSFORM[1].xyz *= scale.y;
	TRANSFORM[2].xyz *= scale.z;
	// Called every frame on existing particles (according to the Fixed FPS property).
}
