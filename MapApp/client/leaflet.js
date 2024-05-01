document.addEventListener('DOMContentLoaded', () => {

  const map = L.map('map').setView([37.05612, 29.10999], 13);
  L.tileLayer('https://{s}.tile.openstreetmap.org/{z}/{x}/{y}.png', {
    maxZoom: 19,
  }).addTo(map);

  let markers = [];
  loadFromDatabase();

  document.getElementById('saveBtn').addEventListener('click', async () => {
    const center = map.getCenter();
    const datetime = new Date().toISOString();
    const point = { id: markers.length, lat: center.lat, lng: center.lng, datetime };
  
    try {
      const response = await fetch('https://localhost:44326/api/Map/Save', {
        method: 'POST',
        headers: {
          'Content-Type': 'application/json',
        },
        body: JSON.stringify(point),
      });
  
      if (response.ok) {
        markers.push(point);
        updatePointList();
      } else {
        console.error('Error saving point:', response.statusText);
      }
    } catch (error) {
      console.error('Error saving point:', error);
    }
  });


  document.getElementById('downloadBtn').addEventListener('click', async () => {
    try {
      const response = await fetch('https://localhost:44326/api/map/download');
  
      if (response.ok) {
        const data = await response.json();
        downloadJSON(data);
      } else {
        console.error('Error downloading points:', response.statusText);
      }
    } catch (error) {
      console.error('Error downloading points:', error);
    }
  });

  async function deletePoint(idToDelete) {
    try {
      const response = await fetch(`https://localhost:44326/api/Map/${idToDelete}`, {
            method: 'DELETE',
        });

        if (response.ok) {
    
            const indexToDelete = markers.findIndex(point => point.id === idToDelete);
            markers.splice(indexToDelete, 1); // Markers dizisinden sil
            updatePointList();
        } else {
            console.error('Error deleting point:', response.statusText);
        }
    } catch (error) {
        console.error('Error deleting point:', error);
    }
  }

  async function loadFromDatabase() {
  try {
    const response = await fetch('https://localhost:44326/api/Map/Points');
    if (response.ok) {
      const data = await response.json();
      markers = data; 
      updatePointList();
    } else {
      console.error('Error loading points:', response.statusText);
    }
  } catch (error) {
    console.error('Error loading points:', error);
  }
}



  
  function updatePointList() {
    const pointList = document.getElementById('pointList');
    pointList.innerHTML = '';
    markers.forEach(point => {
      const li = document.createElement('li');
      li.textContent = `(${point.lat.toFixed(5)}, ${point.lng.toFixed(5)}) - ${point.datetime}`;
      li.addEventListener('click', () => {
        showMarker(point.lat, point.lng);
      });
      const deleteBtn = document.createElement('button');
        deleteBtn.textContent = 'Sil';
        deleteBtn.addEventListener('click', async () => {
              deletePoint(point.id);
        });
      
        li.appendChild(deleteBtn); 
      pointList.appendChild(li);
    });
  }


  function showMarker(lat, lng) {
    const existingMarker = markers.find(point => point.lat === lat && point.lng === lng);
    if (existingMarker) {
      const marker = L.marker([lat, lng]).addTo(map);
      setTimeout(() => {
        map.removeLayer(marker);
      }, 3000);
    }
  }


  function downloadJSON(data) {
    const jsonContent = JSON.stringify(data, null, 2);
    const blob = new Blob([jsonContent], { type: 'application/json' });
    const url = URL.createObjectURL(blob);
    const a = document.createElement('a');
    a.href = url;
    a.download = 'points.json';
    a.click();
    URL.revokeObjectURL(url);
  }
});
