use super::*;
fn body(shape: Shape, position: [f64; 3], half: [f64; 3], dynamic: bool) -> Body {
    Body {
        position,
        velocity: [0.0; 3],
        rotation: [0.0, 0.0, 0.0, 1.0],
        angular_velocity: [0.0; 3],
        freeze_rotation: true,
        gravity: [0.0, -9.81, 0.0],
        linear_damping: 0.0,
        angular_damping: 0.0,
        kinematic: false,
        sleeping: false,
        constraints: 0,
        continuous: false,
        half,
        inverse_mass: if dynamic { 1.0 } else { 0.0 },
        restitution: 0.0,
        friction: 0.5,
        gravity_scale: 1.0,
        dimension: 3,
        shape,
        trigger: false,
        layer: 1,
        mask: u32::MAX,
    }
}
#[test]
fn capsule_narrowphase_does_not_use_its_bounding_box() {
    let capsule = body(Shape::Capsule, [0.0; 3], [0.5, 1.0, 0.5], true);
    let corner = body(Shape::Box, [0.6, 1.0, 0.6], [0.2; 3], false);
    assert!(collision(&capsule, &corner, axes(&capsule), axes(&corner)).is_none());
    let near = body(Shape::Sphere, [0.8, 0.5, 0.0], [0.4; 3], true);
    let contact = collision(&capsule, &near, axes(&capsule), axes(&near)).unwrap();
    assert!((contact.1 - 0.1).abs() < 1e-12);
    assert!((contact.0[0] - 1.0).abs() < 1e-12);
    let far = body(Shape::Capsule, [0.8, 0.0, 0.8], [0.5, 1.0, 0.5], true);
    assert!(collision(&capsule, &far, axes(&capsule), axes(&far)).is_none());
}
#[test]
fn capsules_use_rotated_segments_and_hemispherical_endcaps() {
    let mut capsule = body(Shape::Capsule, [0.0; 3], [0.5, 1.0, 0.5], true);
    capsule.rotation = [
        0.0,
        0.0,
        (std::f64::consts::FRAC_PI_4).sin(),
        (std::f64::consts::FRAC_PI_4).cos(),
    ];
    let sphere = body(Shape::Sphere, [1.2, 0.0, 0.0], [0.3; 3], true);
    let contact = collision(&capsule, &sphere, axes(&capsule), axes(&sphere)).unwrap();
    assert!((contact.1 - 0.1).abs() < 1e-12);
    let mut world = World::default();
    world.add(capsule).unwrap();
    let hit = world
        .raycast([3.0, 0.0, 0.0], [-1.0, 0.0, 0.0], 10.0, 3, 1)
        .unwrap();
    assert!((hit.1 - 2.0).abs() < 1e-12);
    assert!(
        world
            .raycast([3.0, 0.49, 0.49], [-1.0, 0.0, 0.0], 10.0, 3, 1)
            .is_none()
    );
}
#[test]
fn primitive_capsule_falls_and_rests_on_zero_thickness_plane() {
    let mut world = World::default();
    world
        .add(body(Shape::Plane, [0.0; 3], [5.0, 0.0, 5.0], false))
        .unwrap();
    world
        .add(body(Shape::Capsule, [0.0, 3.0, 0.0], [0.5, 1.0, 0.5], true))
        .unwrap();
    for _ in 0..240 {
        world.step();
    }
    assert!(
        (world.bodies[1].position[1] - 1.0).abs() < 0.03,
        "{:?}",
        world.bodies[1].position
    );
    assert!(world.bodies[1].velocity[1].abs() < 0.01);
    assert!(!world.contacts.is_empty());
}
#[test]
fn primitive_surfaces_are_finite_and_one_sided_for_contacts_and_rays() {
    let plane = body(Shape::Plane, [0.0; 3], [5.0, 0.0, 5.0], false);
    let below = body(Shape::Sphere, [0.0, -0.2, 0.0], [0.5; 3], true);
    let outside = body(Shape::Sphere, [6.0, 0.2, 0.0], [0.5; 3], true);
    assert!(collision(&plane, &below, axes(&plane), axes(&below)).is_none());
    assert!(collision(&plane, &outside, axes(&plane), axes(&outside)).is_none());
    let mut world = World::default();
    world.add(plane).unwrap();
    assert_eq!(
        world.raycast([0.0, 2.0, 0.0], [0.0, -1.0, 0.0], 3.0, 3, 1),
        Some((0, 2.0))
    );
    assert!(
        world
            .raycast([0.0, -2.0, 0.0], [0.0, 1.0, 0.0], 3.0, 3, 1)
            .is_none()
    );
    assert!(
        world
            .raycast([6.0, 2.0, 0.0], [0.0, -1.0, 0.0], 3.0, 3, 1)
            .is_none()
    );
    world.bodies.clear();
    world
        .add(body(Shape::Quad, [0.0; 3], [0.5, 0.5, 0.0], false))
        .unwrap();
    assert_eq!(
        world.raycast([0.0, 0.0, -2.0], [0.0, 0.0, 1.0], 3.0, 3, 1),
        Some((0, 2.0))
    );
    assert!(
        world
            .raycast([0.0, 0.0, 2.0], [0.0, 0.0, -1.0], 3.0, 3, 1)
            .is_none()
    );
}
#[test]
fn primitive_surface_and_capsule_validation_rejects_unsupported_shape_data() {
    let mut world = World::default();
    assert!(
        world
            .add(body(Shape::Plane, [0.0; 3], [5.0, 0.1, 5.0], false))
            .is_err()
    );
    assert!(
        world
            .add(body(Shape::Quad, [0.0; 3], [0.5, 0.5, 0.0], true))
            .is_err()
    );
    let mut capsule = body(Shape::Capsule, [0.0; 3], [0.5, 1.0, 0.5], false);
    capsule.dimension = 2;
    assert!(world.add(capsule).is_err());
    assert!(world.bodies.is_empty());
}
