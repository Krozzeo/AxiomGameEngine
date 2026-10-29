use super::REGISTRY;
use axiom_core::physics::{Body, ColliderShape, Shape, World};
#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_clear(id: u32) -> u32 {
    REGISTRY.with_borrow_mut(|r| {
        if !r.worlds.contains_key(&id) {
            return 1;
        }
        r.physics.insert(id, World::default());
        0
    })
}
/// Additional geometry shares its owner's mass and motion. The slot is an
/// integration detail; the host maps it back to the logical component handle.
#[unsafe(no_mangle)]
#[allow(clippy::too_many_arguments)]
pub extern "C" fn axiom_physics_collider(
    id: u32,
    owner: u32,
    slot: u32,
    shape: u32,
    hx: f64,
    hy: f64,
    hz: f64,
    cx: f64,
    cy: f64,
    cz: f64,
    qx: f64,
    qy: f64,
    qz: f64,
    qw: f64,
    trigger: u32,
    layer: u32,
    mask: u32,
    enabled: u32,
) -> u32 {
    let shape = match shape {
        0 => Shape::Box,
        1 => Shape::Sphere,
        2 => Shape::Capsule,
        3 => Shape::Plane,
        4 => Shape::Quad,
        _ => return 2,
    };
    if trigger > 1 || enabled > 1 {
        return 2;
    }
    REGISTRY.with_borrow_mut(|registry| {
        let Some(world) = registry.physics.get_mut(&id) else {
            return 1;
        };
        if world
            .set_collider(
                owner as usize,
                slot as usize,
                ColliderShape {
                    shape,
                    half: [hx, hy, hz],
                    center: [cx, cy, cz],
                    rotation: [qx, qy, qz, qw],
                    trigger: trigger == 1,
                    layer,
                    mask,
                    enabled: enabled == 1,
                },
            )
            .is_ok()
        {
            0
        } else {
            2
        }
    })
}
#[unsafe(no_mangle)]
#[allow(clippy::too_many_arguments)]
pub extern "C" fn axiom_physics_add(
    id: u32,
    dimension: u32,
    shape: u32,
    px: f64,
    py: f64,
    pz: f64,
    vx: f64,
    vy: f64,
    vz: f64,
    hx: f64,
    hy: f64,
    hz: f64,
    inverse_mass: f64,
    restitution: f64,
    friction: f64,
    gravity_scale: f64,
    trigger: u32,
    layer: u32,
    mask: u32,
) -> u32 {
    REGISTRY.with_borrow_mut(|r| {
        if shape > 4 || dimension == 2 && shape > 1 {
            return u32::MAX;
        }
        r.physics
            .get_mut(&id)
            .and_then(|w| {
                w.add(Body {
                    position: [px, py, pz],
                    velocity: [vx, vy, vz],
                    rotation: [0.0, 0.0, 0.0, 1.0],
                    angular_velocity: [0.0; 3],
                    freeze_rotation: false,
                    gravity: [0.0, -9.81, 0.0],
                    linear_damping: 0.0,
                    angular_damping: 0.0,
                    kinematic: false,
                    sleeping: false,
                    constraints: 0,
                    continuous: false,
                    half: [hx, hy, hz],
                    inverse_mass,
                    restitution,
                    friction,
                    gravity_scale,
                    dimension,
                    shape: match shape {
                        1 => Shape::Sphere,
                        2 => Shape::Capsule,
                        3 => Shape::Plane,
                        4 => Shape::Quad,
                        _ => Shape::Box,
                    },
                    trigger: trigger == 1,
                    layer,
                    mask,
                })
                .ok()
            })
            .map_or(u32::MAX, |v| v as u32)
    })
}
#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_step(id: u32, count: u32) -> u32 {
    REGISTRY.with_borrow_mut(|r| {
        let Some(w) = r.physics.get_mut(&id) else {
            return 1;
        };
        if count > 8 {
            return 2;
        }
        for _ in 0..count {
            w.step();
        }
        0
    })
}
#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_read(id: u32, index: u32, field: u32) -> f64 {
    REGISTRY.with_borrow(|r| {
        r.physics
            .get(&id)
            .and_then(|w| w.bodies.get(index as usize))
            .map_or(f64::NAN, |b| match field {
                0..=2 => b.position[field as usize],
                3..=5 => b.velocity[field as usize - 3],
                6..=9 => b.rotation[field as usize - 6],
                10..=12 => b.angular_velocity[field as usize - 10],
                21 => {
                    if b.sleeping {
                        1.0
                    } else {
                        0.0
                    }
                }
                _ => f64::NAN,
            })
    })
}
#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_write(id: u32, index: u32, field: u32, value: f64) -> u32 {
    if !value.is_finite() || value.abs() > if field < 3 { 1e6 } else { 1e4 } {
        return 2;
    }
    REGISTRY.with_borrow_mut(|r| {
        let Some(b) = r
            .physics
            .get_mut(&id)
            .and_then(|w| w.bodies.get_mut(index as usize))
        else {
            return 1;
        };
        match field {
            0..=2 => b.position[field as usize] = value,
            3..=5 => b.velocity[field as usize - 3] = value,
            10..=12 => b.angular_velocity[field as usize - 10] = value,
            13 => b.freeze_rotation = value != 0.0,
            14..=16 => b.gravity[field as usize - 14] = value,
            17 if value >= 0.0 => b.linear_damping = value,
            18 if value >= 0.0 => b.angular_damping = value,
            19 => b.kinematic = value != 0.0,
            20 if (0.0..=126.0).contains(&value) => b.constraints = value as u32,
            21 => b.sleeping = value != 0.0,
            22 => b.continuous = value != 0.0,
            _ => return 2,
        };
        0
    })
}
#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_info(id: u32, field: u32) -> u32 {
    REGISTRY.with_borrow(|r| {
        r.physics.get(&id).map_or(0, |w| match field {
            0 => w.bodies.len() as u32,
            1 => w.contacts.len() as u32,
            2 => w.candidates as u32,
            _ => w.steps as u32,
        })
    })
}
#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_contact(id: u32, index: u32, field: u32) -> f64 {
    REGISTRY.with_borrow(|r| {
        r.physics
            .get(&id)
            .and_then(|w| w.contacts.get(index as usize))
            .map_or(f64::NAN, |c| match field {
                0 => c.a as f64,
                1 => c.b as f64,
                2 => {
                    if c.trigger {
                        1.0
                    } else {
                        0.0
                    }
                }
                3 => c.depth,
                4..=6 => c.normal[field as usize - 4],
                7..=9 => c.point[field as usize - 7],
                10..=12 => c.relative_velocity[field as usize - 10],
                13 => c.impulse,
                14 => c.collider_a as f64,
                15 => c.collider_b as f64,
                _ => f64::NAN,
            })
    })
}
#[unsafe(no_mangle)]
#[allow(clippy::too_many_arguments)]
pub extern "C" fn axiom_physics_ray(
    id: u32,
    ox: f64,
    oy: f64,
    oz: f64,
    dx: f64,
    dy: f64,
    dz: f64,
    max: f64,
    dimension: u32,
    mask: u32,
    field: u32,
) -> f64 {
    REGISTRY.with_borrow(|r| {
        r.physics
            .get(&id)
            .and_then(|w| w.raycast_collider([ox, oy, oz], [dx, dy, dz], max, dimension, mask))
            .map_or(-1.0, |(owner, slot, distance)| match field {
                0 => owner as f64,
                1 => distance,
                2 => slot as f64,
                _ => f64::NAN,
            })
    })
}

#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_rotation(
    id: u32,
    index: u32,
    x: f64,
    y: f64,
    z: f64,
    w: f64,
) -> u32 {
    let q = [x, y, z, w];
    let norm = q.iter().map(|v| v * v).sum::<f64>().sqrt();
    if !norm.is_finite() || norm < 1e-8 {
        return 2;
    }
    REGISTRY.with_borrow_mut(|r| {
        let Some(b) = r
            .physics
            .get_mut(&id)
            .and_then(|w| w.bodies.get_mut(index as usize))
        else {
            return 1;
        };
        b.rotation = if b.dimension == 2 {
            let angle = 2.0 * z.atan2(w);
            [0.0, 0.0, (angle * 0.5).sin(), (angle * 0.5).cos()]
        } else {
            q.map(|v| v / norm)
        };
        0
    })
}

#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_ignore_pair(id: u32, a: u32, b: u32, ignore: u32) -> u32 {
    REGISTRY.with_borrow_mut(|r| {
        let Some(w) = r.physics.get_mut(&id) else {
            return 1;
        };
        if a as usize >= w.bodies.len() || b as usize >= w.bodies.len() {
            return 2;
        }
        let pair = (a.min(b) as usize, a.max(b) as usize);
        if ignore != 0 {
            w.ignored_pairs.insert(pair);
        } else {
            w.ignored_pairs.remove(&pair);
        }
        0
    })
}

#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_ignore_colliders(
    id: u32,
    a: u32,
    slot_a: u32,
    b: u32,
    slot_b: u32,
    ignore: u32,
) -> u32 {
    REGISTRY.with_borrow_mut(|r| {
        let Some(w) = r.physics.get_mut(&id) else {
            return 1;
        };
        if ignore > 1
            || w.collider_count(a as usize)
                .is_none_or(|count| slot_a as usize >= count)
            || w.collider_count(b as usize)
                .is_none_or(|count| slot_b as usize >= count)
        {
            return 2;
        }
        let aa = (a as usize, slot_a as usize);
        let bb = (b as usize, slot_b as usize);
        let pair = if aa < bb { (aa, bb) } else { (bb, aa) };
        if ignore == 1 {
            w.ignored_collider_pairs.insert(pair);
        } else {
            w.ignored_collider_pairs.remove(&pair);
        }
        0
    })
}

#[unsafe(no_mangle)]
pub extern "C" fn axiom_physics_grounded(id: u32, index: u32) -> u32 {
    REGISTRY.with_borrow(|r| {
        r.physics
            .get(&id)
            .and_then(|w| w.grounded.get(index as usize))
            .copied()
            .unwrap_or(false) as u32
    })
}

#[cfg(test)]
mod compound_tests {
    use super::*;
    use crate::exports::{axiom_create, axiom_destroy};

    fn add(id: u32, position: [f64; 3], layer: u32) -> u32 {
        axiom_physics_add(
            id,
            3,
            0,
            position[0],
            position[1],
            position[2],
            0.0,
            0.0,
            0.0,
            0.5,
            0.5,
            0.5,
            0.0,
            0.0,
            0.5,
            0.0,
            0,
            layer,
            u32::MAX,
        )
    }
    fn shape(id: u32, enabled: u32) -> u32 {
        axiom_physics_collider(
            id,
            0,
            1,
            1,
            0.5,
            0.5,
            0.5,
            2.0,
            0.0,
            0.0,
            0.0,
            0.0,
            0.0,
            1.0,
            1,
            1,
            u32::MAX,
            enabled,
        )
    }
    #[test]
    fn compound_scalar_boundary_preserves_owner_slot_contact_and_reset() {
        let id = axiom_create();
        assert_eq!(axiom_physics_clear(id), 0);
        assert_eq!(add(id, [0.0; 3], 0), 0);
        assert_eq!(shape(id, 1), 0);
        let ray =
            |field| axiom_physics_ray(id, 5.0, 0.0, 0.0, -1.0, 0.0, 0.0, 10.0, 3, u32::MAX, field);
        assert_eq!(ray(0), 0.0);
        assert_eq!(ray(1), 2.5);
        assert_eq!(ray(2), 1.0);
        assert_eq!(shape(id, 2), 2);
        assert_eq!(ray(2), 1.0);
        assert_eq!(add(id, [2.0, -0.9, 0.0], 1), 1);
        assert_eq!(axiom_physics_step(id, 1), 0);
        assert!(axiom_physics_info(id, 1) > 0);
        assert_eq!(axiom_physics_contact(id, 0, 14), 1.0);
        assert_eq!(axiom_physics_contact(id, 0, 15), 0.0);
        assert_eq!(axiom_physics_contact(id, 0, 2), 1.0);
        assert_eq!(axiom_physics_clear(id), 0);
        assert_eq!(add(id, [0.0; 3], 0), 0);
        assert_eq!(ray(0), -1.0);
        axiom_destroy(id);
        assert_eq!(shape(id, 1), 1);
    }
}
