// Store scene reference globally
let scene;

function initRobot() {
    scene = new THREE.Scene();
    const camera = new THREE.PerspectiveCamera(75, window.innerWidth / window.innerHeight, 0.1, 1000);
    const renderer = new THREE.WebGLRenderer({ alpha: true, antialias: true });
    renderer.setSize(250, 250);
    renderer.setClearColor(0x000000, 0);
    
    const container = document.getElementById('robot-container');
    if (!container) {
        console.error('robot-container element not found');
        return;
    }
    container.appendChild(renderer.domElement);

    // Robot head
    const head = new THREE.Mesh(
        new THREE.SphereGeometry(1, 32, 32),
        new THREE.MeshBasicMaterial({ 
            color: 0x00ff00,
            transparent: true,
            opacity: 0.8
        })
    );
    scene.add(head);

    // Eyes
    const leftEye = createEye(-0.3, 0.2);
    const rightEye = createEye(0.3, 0.2);
    scene.add(leftEye);
    scene.add(rightEye);

    // Mouth with explicit name
    const mouth = createMouth();
    mouth.name = 'mouth'; // Critical for animation control
    scene.add(mouth);

    camera.position.z = 3;

    // Mouse tracking
    document.addEventListener('mousemove', trackMouse);

    function animate() {
        requestAnimationFrame(animate);
        head.position.y = Math.sin(Date.now() * 0.002) * 0.05;
        renderer.render(scene, camera);
    }
    animate();
}

function createEye(x, y) {
    const eye = new THREE.Mesh(
        new THREE.SphereGeometry(0.15, 16, 16),
        new THREE.MeshBasicMaterial({ color: 0x111111 })
    );
    eye.position.set(x, y, 0.9);
    return eye;
}

function createMouth() {
    const curve = new THREE.EllipseCurve(0, -0.3, 0.4, 0.1, 0, Math.PI);
    return new THREE.Line(
        new THREE.BufferGeometry().setFromPoints(curve.getPoints(32)),
        new THREE.LineBasicMaterial({ color: 0x111111 })
    );
}

function trackMouse(e) {
    const x = (e.clientX / window.innerWidth) * 2 - 1;
    const y = -(e.clientY / window.innerHeight) * 2 + 1;
    
    const eyes = scene.children.filter(obj => obj.geometry?.type === 'SphereGeometry');
    eyes[0].position.x = -0.3 + x * 0.1;
    eyes[1].position.x = 0.3 + x * 0.1;
    eyes[0].position.y = 0.2 + y * 0.1;
    eyes[1].position.y = 0.2 + y * 0.1;
}

// Blazor interop
window.robotInterop = {
    init: initRobot,
    setTalking: function(isTalking) {
        const mouth = scene?.getObjectByName('mouth');
        if (mouth) {
            mouth.scale.y = isTalking ? 1.5 : 1;
        }
    }
};