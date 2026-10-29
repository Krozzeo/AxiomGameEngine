use super::*;

/// An additional collider belongs to an existing body. It never receives its
/// own integration, velocity or mass, and cannot collide with its siblings.
#[derive(Clone, Debug, PartialEq)]
pub struct ColliderShape {
    pub shape: Shape,
    pub half: [f64; 3],
    pub center: [f64; 3],
    pub rotation: [f64; 4],
    pub trigger: bool,
    pub layer: u32,
    pub mask: u32,
    pub enabled: bool,
}

impl Default for ColliderShape {
    fn default() -> Self {
        Self {
            shape: Shape::Box,
            half: [0.5; 3],
            center: [0.0; 3],
            rotation: [0.0, 0.0, 0.0, 1.0],
            trigger: false,
            layer: 1,
            mask: u32::MAX,
            enabled: true,
        }
    }
}

impl ColliderShape {
    pub(super) fn proxy(&self, owner: &Body) -> Body {
        let mut proxy = owner.clone();
        proxy.position = add(owner.position, rotate(owner.rotation, self.center));
        let a = owner.rotation;
        let b = self.rotation;
        proxy.rotation = [
            a[3] * b[0] + a[0] * b[3] + a[1] * b[2] - a[2] * b[1],
            a[3] * b[1] - a[0] * b[2] + a[1] * b[3] + a[2] * b[0],
            a[3] * b[2] + a[0] * b[1] - a[1] * b[0] + a[2] * b[3],
            a[3] * b[3] - a[0] * b[0] - a[1] * b[1] - a[2] * b[2],
        ];
        proxy.shape = self.shape;
        proxy.half = self.half;
        proxy.trigger = self.trigger;
        proxy.layer = if self.enabled { self.layer } else { 0 };
        proxy.mask = if self.enabled { self.mask } else { 0 };
        proxy
    }
}

impl World {
    /// Slot0 is the existing body's collider; slots1..7 are additional shapes.
    /// Reconfiguration preserves the body's velocity and integration state.
    pub fn set_collider(
        &mut self,
        owner: usize,
        slot: usize,
        mut shape: ColliderShape,
    ) -> Result<(), &'static str> {
        let body = self.bodies.get(owner).ok_or("AX_PHYSICS_0001")?;
        let count = self.collider_shapes.get(&owner).map_or(0, Vec::len);
        let norm = dot4(shape.rotation);
        if slot == 0
            || slot > 7
            || slot > count + 1
            || !norm.is_finite()
            || norm < 1e-12
            || shape.center.iter().any(|v| !v.is_finite() || v.abs() > 1e6)
            || body.dimension == 2
                && (shape.rotation[0].abs() > 1e-12 || shape.rotation[1].abs() > 1e-12)
        {
            return Err("AX_PHYSICS_0001");
        }
        shape.rotation = shape.rotation.map(|v| v / norm.sqrt());
        // Use the same geometry/dimension/static-surface validation as Body.
        World::default().add(shape.proxy(body))?;
        let values = self.collider_shapes.entry(owner).or_default();
        if slot == values.len() + 1 {
            values.push(shape);
        } else {
            values[slot - 1] = shape;
        }
        Ok(())
    }

    pub fn collider_count(&self, owner: usize) -> Option<usize> {
        self.bodies
            .get(owner)
            .map(|_| 1 + self.collider_shapes.get(&owner).map_or(0, Vec::len))
    }

    pub(super) fn compound_proxies(&self) -> std::collections::BTreeMap<usize, Vec<Body>> {
        self.collider_shapes
            .iter()
            .map(|(&owner, shapes)| {
                (
                    owner,
                    shapes
                        .iter()
                        .map(|shape| shape.proxy(&self.bodies[owner]))
                        .collect(),
                )
            })
            .collect()
    }
}

fn dot4(q: [f64; 4]) -> f64 {
    q.iter().map(|value| value * value).sum()
}
