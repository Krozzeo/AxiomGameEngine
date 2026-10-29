use super::primitive_tests::body;
use super::*;

fn floor() -> Body {
    body(Shape::Box, [0.0; 3], [10.0, 0.5, 10.0], false)
}
fn owner() -> Body {
    let mut value = body(Shape::Box, [0.0, 2.0, 0.0], [0.1; 3], true);
    value.layer = 0;
    value.mask = 0;
    value
}
fn foot() -> ColliderShape {
    ColliderShape {
        center: [0.0, -1.0, 0.0],
        ..Default::default()
    }
}

#[test]
fn extra_collider_contacts_and_impulses_belong_to_one_body() {
    let mut world = World::default();
    world.add(floor()).unwrap();
    let id = world.add(owner()).unwrap();
    world.set_collider(id, 1, foot()).unwrap();
    world.step();
    assert_eq!(world.bodies.len(), 2);
    assert_eq!(world.collider_count(id), Some(2));
    assert!(
        world
            .contacts
            .iter()
            .any(|contact| contact.b == id && contact.collider_b == 1 && !contact.trigger)
    );
    assert!(world.bodies[id].velocity[1] > -0.01);
    assert!(world.grounded[id]);
}

#[test]
fn sibling_colliders_never_self_collide_or_duplicate_gravity() {
    let mut world = World::default();
    let id = world
        .add(body(Shape::Box, [0.0; 3], [0.5; 3], true))
        .unwrap();
    world.set_collider(id, 1, ColliderShape::default()).unwrap();
    for _ in 0..60 {
        world.step();
    }
    assert_eq!(world.bodies.len(), 1);
    assert!(world.contacts.is_empty());
    assert!((world.bodies[id].velocity[1] + 9.81).abs() < 1e-10);
}

#[test]
fn an_extra_trigger_reports_its_slot_without_grounding_or_stopping_its_owner() {
    let mut world = World::default();
    world.add(floor()).unwrap();
    let id = world.add(owner()).unwrap();
    world
        .set_collider(
            id,
            1,
            ColliderShape {
                trigger: true,
                ..foot()
            },
        )
        .unwrap();
    world.step();
    assert!(!world.contacts.is_empty());
    assert!(
        world
            .contacts
            .iter()
            .all(|contact| contact.trigger && contact.collider_b == 1 && contact.impulse == 0.0)
    );
    assert!(!world.grounded[id]);
    assert!((world.bodies[id].velocity[1] + 9.81 / 60.0).abs() < 1e-10);
}

#[test]
fn raycast_retains_the_exact_collider_slot_and_independent_enable_state() {
    let mut world = World::default();
    let mut root = owner();
    root.position = [0.0; 3];
    let id = world.add(root).unwrap();
    let shape = ColliderShape {
        shape: Shape::Sphere,
        center: [2.0, 0.0, 0.0],
        ..Default::default()
    };
    world.set_collider(id, 1, shape.clone()).unwrap();
    assert_eq!(
        world.raycast_collider([5.0, 0.0, 0.0], [-1.0, 0.0, 0.0], 10.0, 3, u32::MAX),
        Some((id, 1, 2.5))
    );
    assert_eq!(
        world.raycast([5.0, 0.0, 0.0], [-1.0, 0.0, 0.0], 10.0, 3, u32::MAX),
        Some((id, 2.5))
    );
    world
        .set_collider(
            id,
            1,
            ColliderShape {
                enabled: false,
                ..shape
            },
        )
        .unwrap();
    assert!(
        world
            .raycast_collider([5.0, 0.0, 0.0], [-1.0, 0.0, 0.0], 10.0, 3, u32::MAX)
            .is_none()
    );
}

#[test]
fn identical_shapes_do_not_double_positional_correction() {
    let mut ordinary = World::default();
    let mut compound = World::default();
    for world in [&mut ordinary, &mut compound] {
        world.add(floor()).unwrap();
        world
            .add(body(Shape::Box, [0.0, 0.9, 0.0], [0.5; 3], true))
            .unwrap();
    }
    compound
        .set_collider(1, 1, ColliderShape::default())
        .unwrap();
    for _ in 0..10 {
        ordinary.step();
        compound.step();
    }
    for axis in 0..3 {
        assert!(
            (ordinary.bodies[1].position[axis] - compound.bodies[1].position[axis]).abs() < 1e-10
        );
        assert!(
            (ordinary.bodies[1].velocity[axis] - compound.bodies[1].velocity[axis]).abs() < 1e-10
        );
    }
}

#[test]
fn malformed_compound_geometry_rejects_before_changing_the_existing_shape() {
    let mut world = World::default();
    let id = world.add(owner()).unwrap();
    world.set_collider(id, 1, foot()).unwrap();
    for (owner, slot, shape) in [
        (99, 1, foot()),
        (id, 0, foot()),
        (id, 8, foot()),
        (id, 3, foot()),
        (
            id,
            1,
            ColliderShape {
                half: [-1.0; 3],
                ..foot()
            },
        ),
        (
            id,
            1,
            ColliderShape {
                center: [f64::NAN, 0.0, 0.0],
                ..foot()
            },
        ),
        (
            id,
            1,
            ColliderShape {
                rotation: [0.0; 4],
                ..foot()
            },
        ),
    ] {
        assert!(world.set_collider(owner, slot, shape).is_err());
    }
    assert_eq!(world.collider_shapes[&id], vec![foot()]);
    assert_eq!(world.collider_count(99), None);
}
