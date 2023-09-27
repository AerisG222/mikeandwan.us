// switch on high precision floats
#ifdef GL_ES
precision highp float;
#endif

attribute float displacement;
attribute vec3 colors;

varying vec3 vNormal;
varying vec3 vColor;

void main()
{
    vColor = colors;
    vNormal = normal;

    vec3 newPosition = position + normal * vec3(displacement);

    gl_Position = projectionMatrix * modelViewMatrix * vec4(newPosition,1.0);
}
