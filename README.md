this is my 3D rendering program, that I wrote from the groound up. after building, running the exe as-is will display the debug cube.
however dragging and dropping an OBJ file, or a tri file (moddified OBJ file) will instead load that object into the 3D renderer.
the way in which the tri files are different to OBJ files is that there is two new fields 'b' for background and 'm' for the material.
the background only has values for RGB. but the material has RGB as well as glow, metalic and opacity.
